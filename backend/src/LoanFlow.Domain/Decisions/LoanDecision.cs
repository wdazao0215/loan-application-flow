namespace LoanFlow.Domain.Decisions;

public sealed class LoanDecision(IReadOnlyList<DenialReason> denialReasons)
{
    public IReadOnlyList<DenialReason> DenialReasons { get; } = denialReasons;

    public bool IsApproved => DenialReasons.Count == 0;
}
