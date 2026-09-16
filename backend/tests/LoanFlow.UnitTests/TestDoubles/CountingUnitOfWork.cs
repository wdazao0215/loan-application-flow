using LoanFlow.Application.Abstractions;

namespace LoanFlow.UnitTests.TestDoubles;

internal sealed class CountingUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        return Task.CompletedTask;
    }
}
