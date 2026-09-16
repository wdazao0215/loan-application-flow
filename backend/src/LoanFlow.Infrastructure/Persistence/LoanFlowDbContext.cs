using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Customers;
using LoanFlow.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LoanFlow.Infrastructure.Persistence;

internal sealed class LoanFlowDbContext(DbContextOptions<LoanFlowDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConcurrentSubmissionException(exception);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanFlowDbContext).Assembly);
}
