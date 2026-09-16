using LoanFlow.Domain.Applicants;

namespace LoanFlow.Domain.Decisions.Rules;

public sealed class BlacklistedSsnRule(ISsnBlacklist blacklist) : IDenialRule
{
    public static readonly DenialReason Reason =
        new("identity-not-verified", "We could not verify the identity information you provided.");

    public async Task<DenialReason?> EvaluateAsync(LoanRequest request, CancellationToken cancellationToken) =>
        await blacklist.ContainsAsync(request.Ssn, cancellationToken) ? Reason : null;
}
