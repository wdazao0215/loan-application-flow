using LoanFlow.Domain.Decisions;

namespace LoanFlow.Application.SubmitLoanApplication;

public abstract record SubmitLoanApplicationResult
{
    private SubmitLoanApplicationResult()
    {
    }

    public sealed record Approved(Guid ApplicationId, bool IsReturningCustomer) : SubmitLoanApplicationResult;

    public sealed record Denied(IReadOnlyList<DenialReason> Reasons) : SubmitLoanApplicationResult;
}
