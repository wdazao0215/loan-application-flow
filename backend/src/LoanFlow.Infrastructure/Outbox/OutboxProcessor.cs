using System.Text.Json;
using LoanFlow.Application.Abstractions;
using LoanFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoanFlow.Infrastructure.Outbox;

internal sealed partial class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxOptions> options,
    TimeProvider timeProvider,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private readonly OutboxOptions _options = options.Value;

    public async Task ProcessPendingAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LoanFlowDbContext>();
        var now = timeProvider.GetUtcNow();

        var dueMessages = await dbContext.OutboxMessages
            .Where(message => message.ProcessedAt == null
                && message.Attempts < _options.MaxAttempts
                && message.NextAttemptAt <= now)
            .OrderBy(message => message.OccurredAt)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in dueMessages)
        {
            await DeliverAsync(message, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_options.PollingInterval, timeProvider);

        do
        {
            try
            {
                await ProcessPendingAsync(stoppingToken);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                LogPollingFailed(exception);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task DeliverAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await using var handlerScope = scopeFactory.CreateAsyncScope();
            await DispatchAsync(message, handlerScope.ServiceProvider, cancellationToken);
            message.MarkProcessed(timeProvider.GetUtcNow());
            LogDelivered(message.Type, message.Id);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            message.RecordFailedAttempt(exception.Message, timeProvider.GetUtcNow());
            LogFailedAttempt(message, exception);
        }
    }

    private void LogFailedAttempt(OutboxMessage message, Exception exception)
    {
        if (message.Attempts < _options.MaxAttempts)
        {
            LogDeliveryWillBeRetried(message.Type, message.Id, message.Attempts, _options.MaxAttempts, message.NextAttemptAt, exception.Message);
        }
        else
        {
            LogDeliveryAbandoned(exception, message.Type, message.Id, message.Attempts);
        }
    }

    private static Task DispatchAsync(OutboxMessage message, IServiceProvider services, CancellationToken cancellationToken)
    {
        var eventType = typeof(IEventHandler<>).Assembly.GetType(message.Type)
            ?? throw new InvalidOperationException($"Unknown event type '{message.Type}'.");
        var @event = JsonSerializer.Deserialize(message.Payload, eventType)!;
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handler = services.GetRequiredService(handlerType);

        return (Task)handlerType
            .GetMethod(nameof(IEventHandler<>.HandleAsync))!
            .Invoke(handler, [@event, cancellationToken])!;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Delivered {EventType} from outbox message {MessageId}")]
    private partial void LogDelivered(string eventType, Guid messageId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Delivering {EventType} from outbox message {MessageId} failed on attempt {Attempts} of {MaxAttempts}, retrying at {NextAttemptAt}: {Error}")]
    private partial void LogDeliveryWillBeRetried(string eventType, Guid messageId, int attempts, int maxAttempts, DateTimeOffset nextAttemptAt, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Gave up delivering {EventType} from outbox message {MessageId} after {Attempts} attempts")]
    private partial void LogDeliveryAbandoned(Exception exception, string eventType, Guid messageId, int attempts);

    [LoggerMessage(Level = LogLevel.Error, Message = "Polling the outbox failed")]
    private partial void LogPollingFailed(Exception exception);
}
