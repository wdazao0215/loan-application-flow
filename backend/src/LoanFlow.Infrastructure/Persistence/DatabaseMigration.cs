using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.Infrastructure.Persistence;

public static class DatabaseMigration
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<LoanFlowDbContext>().Database.MigrateAsync();
    }
}
