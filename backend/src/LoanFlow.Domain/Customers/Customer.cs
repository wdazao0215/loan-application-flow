using LoanFlow.Domain.Applicants;

namespace LoanFlow.Domain.Customers;

public sealed class Customer
{
    private Customer(Guid id, Ssn ssn)
    {
        Id = id;
        Ssn = ssn;
    }

    public Guid Id { get; private set; }

    public Ssn Ssn { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string CompanyName { get; private set; } = string.Empty;

    public Address Address { get; private set; } = null!;

    public LoanApplication Application { get; private set; } = null!;

    public static Customer Register(LoanRequest request)
    {
        var customer = new Customer(Guid.CreateVersion7(), request.Ssn);
        customer.TakeApplicantDetailsFrom(request);
        customer.Application = LoanApplication.Open(customer.Id, request.RequestedAmount);
        return customer;
    }

    public void Resubmit(LoanRequest request)
    {
        if (request.Ssn != Ssn)
        {
            throw new DomainValidationException("A resubmission must carry the customer's own SSN.");
        }

        TakeApplicantDetailsFrom(request);
        Application.ChangeRequestedAmount(request.RequestedAmount);
    }

    private void TakeApplicantDetailsFrom(LoanRequest request)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        CompanyName = request.CompanyName;
        Address = request.Address;
    }
}
