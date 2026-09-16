using LoanFlow.Domain.Customers;

namespace LoanFlow.Application.Abstractions;

public interface IExternalLoanService
{
    Task CreateAsync(Customer customer, CancellationToken cancellationToken);

    Task UpdateAsync(Customer customer, CancellationToken cancellationToken);
}
