using LoanFlow.Application.Abstractions;
using LoanFlow.Domain.Decisions;
using LoanFlow.Infrastructure.Blacklist;
using LoanFlow.Infrastructure.ExternalService;
using LoanFlow.Infrastructure.Outbox;
using LoanFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LoanFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddDbContext<LoanFlowDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString("LoanFlow"))
            .UseSnakeCaseNamingConvention());
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<LoanFlowDbContext>());
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.Configure<SsnBlacklistOptions>(configuration.GetSection(SsnBlacklistOptions.SectionName));
        services.AddSingleton<ISsnBlacklist, ConfiguredSsnBlacklist>();

        services.AddScoped<IEventPublisher, OutboxEventPublisher>();
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
        services.AddSingleton<OutboxProcessor>();
        services.AddHostedService(provider => provider.GetRequiredService<OutboxProcessor>());

        services.AddOptions<ExternalServiceOptions>()
            .Bind(configuration.GetSection(ExternalServiceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHttpClient<IExternalLoanService, ExternalLoanServiceClient>((provider, httpClient) =>
        {
            var externalService = provider.GetRequiredService<IOptions<ExternalServiceOptions>>().Value;
            httpClient.BaseAddress = externalService.BaseUrl;
            httpClient.Timeout = externalService.Timeout;
        });

        return services;
    }
}
