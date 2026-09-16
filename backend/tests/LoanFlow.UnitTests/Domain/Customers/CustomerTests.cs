using LoanFlow.Domain;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;
using LoanFlow.UnitTests.TestDoubles;

namespace LoanFlow.UnitTests.Domain.Customers;

public class CustomerTests
{
    [Fact]
    public void Registering_opens_the_loan_application_for_the_customer()
    {
        var request = LoanRequests.Valid(requestedAmount: 25_000m);

        var customer = Customer.Register(request);

        customer.Ssn.ShouldBe(request.Ssn);
        customer.FirstName.ShouldBe(request.FirstName);
        customer.Address.ShouldBe(request.Address);
        customer.Application.CustomerId.ShouldBe(customer.Id);
        customer.Application.RequestedAmount.ShouldBe(new LoanAmount(25_000m));
    }

    [Fact]
    public void Resubmitting_updates_the_same_customer_and_application()
    {
        var customer = Customer.Register(LoanRequests.Valid(requestedAmount: 25_000m));
        var customerId = customer.Id;
        var applicationId = customer.Application.Id;

        customer.Resubmit(LoanRequests.Valid(requestedAmount: 40_000m, lastName: "Byron", city: "Dallas"));

        customer.Id.ShouldBe(customerId);
        customer.Application.Id.ShouldBe(applicationId);
        customer.LastName.ShouldBe("Byron");
        customer.Address.City.ShouldBe("Dallas");
        customer.Application.RequestedAmount.ShouldBe(new LoanAmount(40_000m));
    }

    [Fact]
    public void Resubmitting_with_another_ssn_is_rejected()
    {
        var customer = Customer.Register(LoanRequests.Valid(ssn: "521-45-7788"));

        Should.Throw<DomainValidationException>(() => customer.Resubmit(LoanRequests.Valid(ssn: "521-45-7789")));
    }
}
