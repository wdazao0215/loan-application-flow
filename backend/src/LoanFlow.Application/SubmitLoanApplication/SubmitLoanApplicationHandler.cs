using LoanFlow.Application.Abstractions;
using LoanFlow.Application.ExternalSync;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;
using LoanFlow.Domain.Decisions;

namespace LoanFlow.Application.SubmitLoanApplication;

public sealed class SubmitLoanApplicationHandler(
    LoanDecisionEngine decisionEngine,
    ICustomerRepository customers,
    IEventPublisher eventPublisher,
    IUnitOfWork unitOfWork)
{
    public async Task<SubmitLoanApplicationResult> HandleAsync(
        SubmitLoanApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.ToLoanRequest();
        var decision = await decisionEngine.DecideAsync(request, cancellationToken);

        if (!decision.IsApproved)
        {
            return new SubmitLoanApplicationResult.Denied(decision.DenialReasons);
        }

        var (customer, isReturningCustomer) = await RegisterOrResubmitAsync(request, cancellationToken);

        eventPublisher.Publish(new LoanApplicationApproved(customer.Id, customer.Application.Id, isReturningCustomer));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitLoanApplicationResult.Approved(customer.Application.Id, isReturningCustomer);
    }

    private async Task<(Customer Customer, bool IsReturningCustomer)> RegisterOrResubmitAsync(
        LoanRequest request,
        CancellationToken cancellationToken)
    {
        var existingCustomer = await customers.FindBySsnAsync(request.Ssn, cancellationToken);

        if (existingCustomer is not null)
        {
            existingCustomer.Resubmit(request);
            return (existingCustomer, true);
        }

        var newCustomer = Customer.Register(request);
        customers.Add(newCustomer);
        return (newCustomer, false);
    }
}
