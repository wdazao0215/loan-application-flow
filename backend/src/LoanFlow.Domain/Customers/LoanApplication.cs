using LoanFlow.Domain.Applicants;

namespace LoanFlow.Domain.Customers;

public sealed class LoanApplication
{
    private LoanApplication(Guid id, Guid customerId, LoanAmount requestedAmount)
    {
        Id = id;
        CustomerId = customerId;
        RequestedAmount = requestedAmount;
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public LoanAmount RequestedAmount { get; private set; }

    internal static LoanApplication Open(Guid customerId, LoanAmount requestedAmount) =>
        new(Guid.CreateVersion7(), customerId, requestedAmount);

    internal void ChangeRequestedAmount(LoanAmount requestedAmount) => RequestedAmount = requestedAmount;
}
