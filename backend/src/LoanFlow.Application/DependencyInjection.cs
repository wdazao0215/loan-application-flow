using LoanFlow.Application.Abstractions;
using LoanFlow.Application.ExternalSync;
using LoanFlow.Application.SubmitLoanApplication;
using LoanFlow.Domain.Decisions;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        foreach (var ruleType in DenialRuleTypes())
        {
            services.AddScoped(typeof(IDenialRule), ruleType);
        }

        services.AddScoped<LoanDecisionEngine>();
        services.AddScoped<SubmitLoanApplicationHandler>();
        services.AddScoped<IEventHandler<LoanApplicationApproved>, SyncApprovedApplicationHandler>();

        return services;
    }

    private static IEnumerable<Type> DenialRuleTypes() =>
        typeof(IDenialRule).Assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsAssignableTo(typeof(IDenialRule)));
}
