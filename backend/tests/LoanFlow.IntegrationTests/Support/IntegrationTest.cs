using System.Net.Http.Json;
using LoanFlow.Api.Contracts;
using LoanFlow.Infrastructure.Outbox;
using LoanFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.IntegrationTests.Support;

[Collection(nameof(ApiCollection))]
public abstract class IntegrationTest(LoanFlowApiFactory factory) : IAsyncLifetime
{
    protected const string LoanApplicationsPath = "/api/loan-applications";

    protected LoanFlowApiFactory Factory { get; } = factory;

    protected HttpClient Client { get; } = factory.CreateClient();

    public virtual async Task InitializeAsync()
    {
        Factory.ExternalService.Reset();
        await ExecuteSqlAsync("TRUNCATE customers, loan_applications, outbox_messages");
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    protected async Task<LoanApplicationDecisionResponse> SubmitAsync(SubmitLoanApplicationRequest request)
    {
        using var response = await Client.PostAsJsonAsync(LoanApplicationsPath, request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LoanApplicationDecisionResponse>())!;
    }

    protected Task ProcessOutboxAsync() =>
        Factory.Services.GetRequiredService<OutboxProcessor>().ProcessPendingAsync(CancellationToken.None);

    protected Task ExecuteSqlAsync(string sql) =>
        QueryAsync(dbContext => dbContext.Database.ExecuteSqlRawAsync(sql));

    private protected async Task<T> QueryAsync<T>(Func<LoanFlowDbContext, Task<T>> query)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        return await query(scope.ServiceProvider.GetRequiredService<LoanFlowDbContext>());
    }
}
