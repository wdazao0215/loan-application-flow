namespace LoanFlow.Infrastructure.Outbox;

internal sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public TimeSpan PollingInterval { get; init; } = TimeSpan.FromSeconds(2);

    public int BatchSize { get; init; } = 20;

    public int MaxAttempts { get; init; } = 10;
}
