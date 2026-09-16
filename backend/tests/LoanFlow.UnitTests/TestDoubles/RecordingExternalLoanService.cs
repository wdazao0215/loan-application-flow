using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Customers;

namespace LoanFlow.UnitTests.TestDoubles;

internal sealed class RecordingExternalLoanService : IExternalLoanService
{
    public List<(string Operation, Customer Customer)> Calls { get; } = [];

    public Task CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        Calls.Add(("create", customer));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        Calls.Add(("update", customer));
        return Task.CompletedTask;
    }
}
