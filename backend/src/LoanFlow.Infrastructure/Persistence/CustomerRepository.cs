using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

internal sealed class CustomerRepository(LoanFlowDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> FindBySsnAsync(Ssn ssn, CancellationToken cancellationToken) =>
        dbContext.Customers.SingleOrDefaultAsync(customer => customer.Ssn == ssn, cancellationToken);

    public Task<Customer?> FindByIdAsync(Guid customerId, CancellationToken cancellationToken) =>
        dbContext.Customers.SingleOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);

    public void Add(Customer customer) => dbContext.Customers.Add(customer);
}
