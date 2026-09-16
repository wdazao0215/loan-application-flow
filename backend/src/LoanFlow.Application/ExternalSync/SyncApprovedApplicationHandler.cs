using LoanFlow.Application.Abstractions;

namespace LoanFlow.Application.ExternalSync;

public sealed class SyncApprovedApplicationHandler(
    ICustomerRepository customers,
    IExternalLoanService externalLoanService) : IEventHandler<LoanApplicationApproved>
{
    public async Task HandleAsync(LoanApplicationApproved @event, CancellationToken cancellationToken)
    {
        var customer = await customers.FindByIdAsync(@event.CustomerId, cancellationToken)
            ?? throw new InvalidOperationException($"Customer {@event.CustomerId} does not exist.");

        if (@event.IsReturningCustomer)
        {
            await externalLoanService.UpdateAsync(customer, cancellationToken);
        }
        else
        {
            await externalLoanService.CreateAsync(customer, cancellationToken);
        }
    }
}
