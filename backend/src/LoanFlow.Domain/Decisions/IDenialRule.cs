using LoanFlow.Domain.Applicants;

namespace LoanFlow.Domain.Decisions;

public interface IDenialRule
{
    Task<DenialReason?> EvaluateAsync(LoanRequest request, CancellationToken cancellationToken);
}
