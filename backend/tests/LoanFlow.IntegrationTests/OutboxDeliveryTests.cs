using System.Net;
using LoanFlow.IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.IntegrationTests;

public class OutboxDeliveryTests(LoanFlowApiFactory factory) : IntegrationTest(factory)
{
    [Fact]
    public async Task Creates_a_new_customer_in_the_external_service()
    {
        var response = await SubmitAsync(Requests.LoanApplication(requestedAmount: 25_000m));

        await ProcessOutboxAsync();

        var request = Factory.ExternalService.Requests.ShouldHaveSingleItem();
        request.Method.ShouldBe(HttpMethod.Post);
        request.Path.ShouldBe("/loan-applications");
        request.Body.GetProperty("applicationId").GetGuid().ShouldBe(response.ApplicationId!.Value);
        request.Body.GetProperty("requestedAmount").GetDecimal().ShouldBe(25_000m);
        request.Body.GetProperty("customer").GetProperty("ssn").GetString().ShouldBe("521457788");
        request.Body.GetProperty("customer").GetProperty("address").GetProperty("state").GetString().ShouldBe("TX");
        (await QueryAsync(db => db.OutboxMessages.AnyAsync(m => m.ProcessedAt == null))).ShouldBeFalse();
    }

    [Fact]
    public async Task Updates_a_returning_customer_in_the_external_service()
    {
        var response = await SubmitAsync(Requests.LoanApplication(requestedAmount: 25_000m));
        await ProcessOutboxAsync();

        await SubmitAsync(Requests.LoanApplication(requestedAmount: 70_000m));
        await ProcessOutboxAsync();

        Factory.ExternalService.Requests.Select(r => (r.Method, r.Path)).ShouldBe(
        [
            (HttpMethod.Post, "/loan-applications"),
            (HttpMethod.Put, $"/loan-applications/{response.ApplicationId}"),
        ]);
        Factory.ExternalService.Requests[1].Body.GetProperty("requestedAmount").GetDecimal().ShouldBe(70_000m);
    }

    [Fact]
    public async Task Keeps_the_event_pending_and_backs_off_when_the_external_service_fails()
    {
        Factory.ExternalService.RespondWith(HttpStatusCode.ServiceUnavailable);
        await SubmitAsync(Requests.LoanApplication());

        await ProcessOutboxAsync();
        await ProcessOutboxAsync();

        Factory.ExternalService.Requests.Count.ShouldBe(1);
        var message = (await QueryAsync(db => db.OutboxMessages.AsNoTracking().ToListAsync())).ShouldHaveSingleItem();
        message.ProcessedAt.ShouldBeNull();
        message.Attempts.ShouldBe(1);
        message.NextAttemptAt.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
        message.LastError.ShouldNotBeNull().ShouldContain("503");
    }

    [Fact]
    public async Task Counts_a_timeout_as_a_failed_attempt()
    {
        Factory.ExternalService.FailWith(new TaskCanceledException("timed out", new TimeoutException()));
        await SubmitAsync(Requests.LoanApplication());

        await ProcessOutboxAsync();

        var message = (await QueryAsync(db => db.OutboxMessages.AsNoTracking().ToListAsync())).ShouldHaveSingleItem();
        message.ProcessedAt.ShouldBeNull();
        message.Attempts.ShouldBe(1);
    }
}
