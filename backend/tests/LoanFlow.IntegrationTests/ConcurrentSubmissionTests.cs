using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;
using LoanFlow.IntegrationTests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.IntegrationTests;

public class ConcurrentSubmissionTests(LoanFlowApiFactory factory) : IntegrationTest(factory)
{
    [Fact]
    public async Task A_second_customer_with_the_same_ssn_is_rejected_as_a_concurrent_submission()
    {
        await using var firstRequest = Factory.Services.CreateAsyncScope();
        await using var secondRequest = Factory.Services.CreateAsyncScope();

        Register(firstRequest);
        Register(secondRequest);
        await firstRequest.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveChangesAsync(CancellationToken.None);

        await Should.ThrowAsync<ConcurrentSubmissionException>(
            () => secondRequest.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveChangesAsync(CancellationToken.None));
    }

    private static void Register(AsyncServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<ICustomerRepository>().Add(Customer.Register(new LoanRequest(
            "Ada",
            "Lovelace",
            "Analytical Engines LLC",
            new Address("100 Congress Ave", "Austin", "TX", "78701"),
            new Ssn(Requests.ApprovableSsn),
            new LoanAmount(25_000m))));
}
