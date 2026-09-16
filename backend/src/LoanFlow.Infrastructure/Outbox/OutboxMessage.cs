using System.Text.Json;

namespace LoanFlow.Infrastructure.Outbox;

internal sealed class OutboxMessage
{
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromMinutes(5);

    private OutboxMessage(Guid id, string type, string payload, DateTimeOffset occurredAt)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredAt = occurredAt;
        NextAttemptAt = occurredAt;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public string Payload { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public int Attempts { get; private set; }

    public DateTimeOffset NextAttemptAt { get; private set; }

    public string? LastError { get; private set; }

    public static OutboxMessage For<TEvent>(TEvent @event, DateTimeOffset occurredAt)
        where TEvent : class =>
        new(Guid.CreateVersion7(), typeof(TEvent).FullName!, JsonSerializer.Serialize(@event), occurredAt);

    public void MarkProcessed(DateTimeOffset processedAt) => ProcessedAt = processedAt;

    public void RecordFailedAttempt(string error, DateTimeOffset failedAt)
    {
        Attempts++;
        LastError = error;
        NextAttemptAt = failedAt + RetryDelayAfter(Attempts);
    }

    private static TimeSpan RetryDelayAfter(int attempts)
    {
        var exponentialDelay = TimeSpan.FromSeconds(Math.Pow(2, attempts));
        return exponentialDelay < MaxRetryDelay ? exponentialDelay : MaxRetryDelay;
    }
}
