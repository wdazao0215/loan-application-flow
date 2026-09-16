using LoanFlow.Domain.Applicants;

namespace LoanFlow.Domain.Decisions.Rules;

public sealed class StateNotServedRule : IDenialRule
{
    private const string NewYork = "NY";

    public static readonly DenialReason Reason =
        new("state-not-served", "We are not able to offer loans in your state yet.");

    public Task<DenialReason?> EvaluateAsync(LoanRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(request.Address.State == NewYork ? Reason : null);
}
