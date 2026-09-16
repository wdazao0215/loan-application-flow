using LoanFlow.Application.ExternalSync;
using LoanFlow.Domain.Customers;
using LoanFlow.UnitTests.TestDoubles;

namespace LoanFlow.UnitTests.Application;

public class SyncApprovedApplicationHandlerTests
{
    private readonly Customer _customer = Customer.Register(LoanRequests.Valid());
    private readonly RecordingExternalLoanService _externalLoanService = new();

    private SyncApprovedApplicationHandler Handler() =>
        new(new InMemoryCustomerRepository(_customer), _externalLoanService);

    [Theory]
    [InlineData(false, "create")]
    [InlineData(true, "update")]
    public async Task Creates_new_customers_and_updates_returning_ones(bool isReturningCustomer, string expectedOperation)
    {
        var @event = new LoanApplicationApproved(_customer.Id, _customer.Application.Id, isReturningCustomer);

        await Handler().HandleAsync(@event, CancellationToken.None);

        _externalLoanService.Calls.ShouldBe([(expectedOperation, _customer)]);
    }

    [Fact]
    public async Task Fails_when_the_customer_does_not_exist_so_the_event_is_retried()
    {
        var @event = new LoanApplicationApproved(Guid.NewGuid(), Guid.NewGuid(), false);

        await Should.ThrowAsync<InvalidOperationException>(() => Handler().HandleAsync(@event, CancellationToken.None));

        _externalLoanService.Calls.ShouldBeEmpty();
    }
}
