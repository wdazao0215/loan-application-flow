using System.Net;
using System.Net.Http.Json;
using LoanFlow.IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.IntegrationTests;

public class TransactionRollbackTests(LoanFlowApiFactory factory) : IntegrationTest(factory)
{
    public override async Task DisposeAsync()
    {
        await ExecuteSqlAsync("DROP TRIGGER IF EXISTS reject_outbox_insert ON outbox_messages");
        await ExecuteSqlAsync("DROP FUNCTION IF EXISTS reject_outbox_insert()");
    }

    [Fact]
    public async Task Rolls_back_a_new_customer_when_the_event_cannot_be_stored()
    {
        await MakeOutboxInsertsFailAsync();

        using var response = await Client.PostAsJsonAsync(LoanApplicationsPath, Requests.LoanApplication());

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        (await QueryAsync(db => db.Customers.CountAsync())).ShouldBe(0);
        (await QueryAsync(db => db.Set<LoanFlow.Domain.Customers.LoanApplication>().CountAsync())).ShouldBe(0);
        (await QueryAsync(db => db.OutboxMessages.CountAsync())).ShouldBe(0);
    }

    [Fact]
    public async Task Rolls_back_a_returning_customer_update_when_the_event_cannot_be_stored()
    {
        await SubmitAsync(Requests.LoanApplication(requestedAmount: 25_000m, lastName: "Lovelace"));
        await MakeOutboxInsertsFailAsync();

        using var response = await Client.PostAsJsonAsync(
            LoanApplicationsPath,
            Requests.LoanApplication(requestedAmount: 90_000m, lastName: "Byron"));

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        var customer = (await QueryAsync(db => db.Customers.AsNoTracking().ToListAsync())).ShouldHaveSingleItem();
        customer.LastName.ShouldBe("Lovelace");
        customer.Application.RequestedAmount.Value.ShouldBe(25_000m);
        (await QueryAsync(db => db.OutboxMessages.CountAsync())).ShouldBe(1);
    }

    private async Task MakeOutboxInsertsFailAsync()
    {
        await ExecuteSqlAsync("""
            CREATE FUNCTION reject_outbox_insert() RETURNS trigger LANGUAGE plpgsql AS $$
            BEGIN
                RAISE EXCEPTION 'outbox is unavailable';
            END;
            $$
            """);
        await ExecuteSqlAsync("""
            CREATE TRIGGER reject_outbox_insert BEFORE INSERT ON outbox_messages
            FOR EACH ROW EXECUTE FUNCTION reject_outbox_insert()
            """);
    }
}
