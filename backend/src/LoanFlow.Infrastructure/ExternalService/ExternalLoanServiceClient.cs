using System.Net.Http.Json;
using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Customers;

namespace LoanFlow.Infrastructure.ExternalService;

internal sealed class ExternalLoanServiceClient(HttpClient httpClient) : IExternalLoanService
{
    private const string LoanApplicationsPath = "loan-applications";

    public async Task CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            LoanApplicationsPath,
            ExternalLoanApplication.From(customer),
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"{LoanApplicationsPath}/{customer.Application.Id}",
            ExternalLoanApplication.From(customer),
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
