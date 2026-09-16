using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LoanFlow.Application.ExternalSync;
using LoanFlow.IntegrationTests.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.IntegrationTests;

public class SubmitLoanApplicationEndpointTests(LoanFlowApiFactory factory) : IntegrationTest(factory)
{
    [Fact]
    public async Task Approves_and_saves_the_customer_the_application_and_the_event_together()
    {
        var response = await SubmitAsync(Requests.LoanApplication(requestedAmount: 25_000m));

        response.Decision.ShouldBe("approved");
        response.IsReturningCustomer.ShouldBe(false);

        var customer = (await QueryAsync(db => db.Customers.AsNoTracking().ToListAsync())).ShouldHaveSingleItem();
        customer.Ssn.Value.ShouldBe("521457788");
        customer.Address.State.ShouldBe("TX");
        customer.Application.Id.ShouldBe(response.ApplicationId!.Value);
        customer.Application.CustomerId.ShouldBe(customer.Id);
        customer.Application.RequestedAmount.Value.ShouldBe(25_000m);

        var message = (await QueryAsync(db => db.OutboxMessages.AsNoTracking().ToListAsync())).ShouldHaveSingleItem();
        message.ProcessedAt.ShouldBeNull();
        JsonSerializer.Deserialize<LoanApplicationApproved>(message.Payload)
            .ShouldBe(new LoanApplicationApproved(customer.Id, customer.Application.Id, IsReturningCustomer: false));
    }

    [Fact]
    public async Task Updates_the_same_customer_and_application_when_the_ssn_returns()
    {
        var first = await SubmitAsync(Requests.LoanApplication(ssn: "521-45-7788", requestedAmount: 25_000m));

        var second = await SubmitAsync(Requests.LoanApplication(ssn: "521457788", requestedAmount: 60_000m, lastName: "Byron"));

        second.Decision.ShouldBe("approved");
        second.IsReturningCustomer.ShouldBe(true);
        second.ApplicationId.ShouldBe(first.ApplicationId);

        var customer = (await QueryAsync(db => db.Customers.AsNoTracking().ToListAsync())).ShouldHaveSingleItem();
        customer.LastName.ShouldBe("Byron");
        customer.Application.RequestedAmount.Value.ShouldBe(60_000m);
        (await QueryAsync(db => db.Set<LoanFlow.Domain.Customers.LoanApplication>().CountAsync())).ShouldBe(1);

        var events = await QueryAsync(db => db.OutboxMessages.AsNoTracking().OrderBy(m => m.OccurredAt).ToListAsync());
        events.Select(m => JsonSerializer.Deserialize<LoanApplicationApproved>(m.Payload)!.IsReturningCustomer)
            .ShouldBe([false, true]);
    }

    [Theory]
    [InlineData("NY", Requests.ApprovableSsn, "state-not-served")]
    [InlineData("TX", Requests.BlacklistedSsn, "identity-not-verified")]
    public async Task Denies_without_saving_or_publishing_anything(string state, string ssn, string expectedCode)
    {
        var response = await SubmitAsync(Requests.LoanApplication(ssn: ssn, state: state));

        response.Decision.ShouldBe("denied");
        response.ApplicationId.ShouldBeNull();
        response.Reasons.ShouldNotBeNull().ShouldHaveSingleItem().Code.ShouldBe(expectedCode);
        (await QueryAsync(db => db.Customers.CountAsync())).ShouldBe(0);
        (await QueryAsync(db => db.OutboxMessages.CountAsync())).ShouldBe(0);
    }

    [Fact]
    public async Task Rejects_invalid_input_with_problem_details()
    {
        using var response = await Client.PostAsJsonAsync(LoanApplicationsPath, Requests.LoanApplication(ssn: "12345"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.ShouldNotBeNull().Detail.ShouldNotBeNull().ShouldContain("SSN");
        (await QueryAsync(db => db.Customers.CountAsync())).ShouldBe(0);
    }
}
