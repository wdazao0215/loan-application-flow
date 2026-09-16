using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;

namespace LoanFlow.Application.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> FindBySsnAsync(Ssn ssn, CancellationToken cancellationToken);

    Task<Customer?> FindByIdAsync(Guid customerId, CancellationToken cancellationToken);

    void Add(Customer customer);
}
