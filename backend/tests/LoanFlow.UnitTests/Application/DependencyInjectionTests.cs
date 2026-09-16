using LoanFlow.Application;
using LoanFlow.Domain.Decisions;
using LoanFlow.Domain.Decisions.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.UnitTests.Application;

public class DependencyInjectionTests
{
    [Fact]
    public void Registers_every_denial_rule_in_the_domain()
    {
        var services = new ServiceCollection().AddApplication();

        services
            .Where(descriptor => descriptor.ServiceType == typeof(IDenialRule))
            .Select(descriptor => descriptor.ImplementationType)
            .ShouldBe([typeof(StateNotServedRule), typeof(BlacklistedSsnRule)], ignoreOrder: true);
    }
}
