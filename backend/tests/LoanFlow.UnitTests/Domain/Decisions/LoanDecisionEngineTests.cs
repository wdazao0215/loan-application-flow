using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Decisions;
using LoanFlow.Domain.Decisions.Rules;
using LoanFlow.UnitTests.TestDoubles;

namespace LoanFlow.UnitTests.Domain.Decisions;

public class LoanDecisionEngineTests
{
    private const string BlacklistedSsn = "123456789";

    private static LoanDecisionEngine EngineWith(params IDenialRule[] extraRules) =>
        new([new StateNotServedRule(), new BlacklistedSsnRule(new InMemorySsnBlacklist(BlacklistedSsn)), .. extraRules]);

    [Fact]
    public async Task Approves_when_no_rule_denies()
    {
        var decision = await EngineWith().DecideAsync(LoanRequests.Valid(state: "TX"), CancellationToken.None);

        decision.IsApproved.ShouldBeTrue();
        decision.DenialReasons.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("NY")]
    [InlineData("ny")]
    public async Task Denies_applicants_from_new_york(string state)
    {
        var decision = await EngineWith().DecideAsync(LoanRequests.Valid(state: state), CancellationToken.None);

        decision.IsApproved.ShouldBeFalse();
        decision.DenialReasons.ShouldBe([StateNotServedRule.Reason]);
    }

    [Theory]
    [InlineData("123-45-6789")]
    [InlineData("123456789")]
    public async Task Denies_blacklisted_ssns_whatever_their_format(string ssn)
    {
        var decision = await EngineWith().DecideAsync(LoanRequests.Valid(ssn: ssn), CancellationToken.None);

        decision.IsApproved.ShouldBeFalse();
        decision.DenialReasons.ShouldBe([BlacklistedSsnRule.Reason]);
    }

    [Fact]
    public async Task Reports_every_rule_that_denies()
    {
        var request = LoanRequests.Valid(state: "NY", ssn: "123-45-6789");

        var decision = await EngineWith().DecideAsync(request, CancellationToken.None);

        decision.DenialReasons.ShouldBe([StateNotServedRule.Reason, BlacklistedSsnRule.Reason], ignoreOrder: true);
    }

    [Fact]
    public async Task Applies_a_new_rule_without_changing_the_existing_ones()
    {
        var engine = EngineWith(new AmountAboveLimitRule(limit: 50_000m));

        var decision = await engine.DecideAsync(LoanRequests.Valid(requestedAmount: 80_000m), CancellationToken.None);

        decision.DenialReasons.ShouldBe([AmountAboveLimitRule.Reason]);
    }

    private sealed class AmountAboveLimitRule(decimal limit) : IDenialRule
    {
        public static readonly DenialReason Reason = new("amount-above-limit", "Requested amount is above the limit.");

        public Task<DenialReason?> EvaluateAsync(LoanRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(request.RequestedAmount.Value > limit ? Reason : null);
    }
}
