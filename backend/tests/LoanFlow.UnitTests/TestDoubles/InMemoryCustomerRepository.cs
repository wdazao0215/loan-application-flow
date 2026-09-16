using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;

namespace LoanFlow.UnitTests.TestDoubles;

internal sealed class InMemoryCustomerRepository(params Customer[] existingCustomers) : ICustomerRepository
{
    public List<Customer> Customers { get; } = [.. existingCustomers];

    public Task<Customer?> FindBySsnAsync(Ssn ssn, CancellationToken cancellationToken) =>
        Task.FromResult(Customers.SingleOrDefault(customer => customer.Ssn == ssn));

    public Task<Customer?> FindByIdAsync(Guid customerId, CancellationToken cancellationToken) =>
        Task.FromResult(Customers.SingleOrDefault(customer => customer.Id == customerId));

    public void Add(Customer customer) => Customers.Add(customer);
}
