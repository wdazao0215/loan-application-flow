using LoanFlow.Application.ExternalSync;
using LoanFlow.Application.SubmitLoanApplication;
using LoanFlow.Domain;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;
using LoanFlow.Domain.Decisions;
using LoanFlow.Domain.Decisions.Rules;
using LoanFlow.UnitTests.TestDoubles;

namespace LoanFlow.UnitTests.Application;

public class SubmitLoanApplicationHandlerTests
{
    private const string BlacklistedSsn = "123-45-6789";

    private readonly RecordingEventPublisher _eventPublisher = new();
    private readonly CountingUnitOfWork _unitOfWork = new();

    private SubmitLoanApplicationHandler HandlerWith(InMemoryCustomerRepository customers) =>
        new(
            new LoanDecisionEngine([new StateNotServedRule(), new BlacklistedSsnRule(new InMemorySsnBlacklist(BlacklistedSsn))]),
            customers,
            _eventPublisher,
            _unitOfWork);

    private static SubmitLoanApplicationCommand Command(
        string ssn = LoanRequests.ApprovableSsn,
        string state = "TX",
        decimal requestedAmount = 25_000m,
        string lastName = "Lovelace") =>
        new("Ada", lastName, "Analytical Engines LLC", "100 Congress Ave", "Austin", state, "78701", ssn, requestedAmount);

    [Fact]
    public async Task Registers_a_new_customer_and_publishes_a_create_event()
    {
        var customers = new InMemoryCustomerRepository();

        var result = await HandlerWith(customers).HandleAsync(Command(), CancellationToken.None);

        var customer = customers.Customers.ShouldHaveSingleItem();
        result.ShouldBe(new SubmitLoanApplicationResult.Approved(customer.Application.Id, IsReturningCustomer: false));
        _eventPublisher.PublishedEvents.ShouldBe([new LoanApplicationApproved(customer.Id, customer.Application.Id, false)]);
        _unitOfWork.SaveCount.ShouldBe(1);
    }

    [Fact]
    public async Task Updates_the_existing_customer_and_application_for_a_returning_ssn()
    {
        var existingCustomer = Customer.Register(LoanRequests.Valid(ssn: "521457788", requestedAmount: 25_000m));
        var customers = new InMemoryCustomerRepository(existingCustomer);

        var result = await HandlerWith(customers)
            .HandleAsync(Command(ssn: "521-45-7788", requestedAmount: 60_000m, lastName: "Byron"), CancellationToken.None);

        customers.Customers.ShouldHaveSingleItem().ShouldBeSameAs(existingCustomer);
        existingCustomer.LastName.ShouldBe("Byron");
        existingCustomer.Application.RequestedAmount.ShouldBe(new LoanAmount(60_000m));
        result.ShouldBe(new SubmitLoanApplicationResult.Approved(existingCustomer.Application.Id, IsReturningCustomer: true));
        _eventPublisher.PublishedEvents.ShouldBe(
            [new LoanApplicationApproved(existingCustomer.Id, existingCustomer.Application.Id, true)]);
        _unitOfWork.SaveCount.ShouldBe(1);
    }

    [Theory]
    [InlineData("NY", LoanRequests.ApprovableSsn, "state-not-served")]
    [InlineData("TX", BlacklistedSsn, "identity-not-verified")]
    public async Task Saves_and_publishes_nothing_when_denied(string state, string ssn, string expectedCode)
    {
        var customers = new InMemoryCustomerRepository();

        var result = await HandlerWith(customers).HandleAsync(Command(ssn: ssn, state: state), CancellationToken.None);

        result.ShouldBeOfType<SubmitLoanApplicationResult.Denied>()
            .Reasons.ShouldHaveSingleItem().Code.ShouldBe(expectedCode);
        customers.Customers.ShouldBeEmpty();
        _eventPublisher.PublishedEvents.ShouldBeEmpty();
        _unitOfWork.SaveCount.ShouldBe(0);
    }

    [Fact]
    public async Task Rejects_invalid_input_before_evaluating_or_saving_anything()
    {
        var customers = new InMemoryCustomerRepository();

        await Should.ThrowAsync<DomainValidationException>(
            () => HandlerWith(customers).HandleAsync(Command(ssn: "12345"), CancellationToken.None));

        customers.Customers.ShouldBeEmpty();
        _unitOfWork.SaveCount.ShouldBe(0);
    }
}
