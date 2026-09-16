using LoanFlow.Domain.Customers;

namespace LoanFlow.Infrastructure.ExternalService;

internal sealed record ExternalLoanApplication(Guid ApplicationId, decimal RequestedAmount, ExternalCustomer Customer)
{
    public static ExternalLoanApplication From(Customer customer) =>
        new(
            customer.Application.Id,
            customer.Application.RequestedAmount.Value,
            new ExternalCustomer(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Ssn.Value,
                customer.CompanyName,
                new ExternalAddress(
                    customer.Address.Street,
                    customer.Address.City,
                    customer.Address.State,
                    customer.Address.ZipCode)));
}

internal sealed record ExternalCustomer(
    Guid Id,
    string FirstName,
    string LastName,
    string Ssn,
    string CompanyName,
    ExternalAddress Address);

internal sealed record ExternalAddress(string Street, string City, string State, string ZipCode);
