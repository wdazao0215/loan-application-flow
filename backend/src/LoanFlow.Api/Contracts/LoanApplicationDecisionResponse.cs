using LoanFlow.Application.SubmitLoanApplication;

namespace LoanFlow.Api.Contracts;

public sealed record LoanApplicationDecisionResponse(
    string Decision,
    Guid? ApplicationId,
    bool? IsReturningCustomer,
    IReadOnlyList<DenialReasonResponse>? Reasons)
{
    public static LoanApplicationDecisionResponse From(SubmitLoanApplicationResult result) =>
        result switch
        {
            SubmitLoanApplicationResult.Approved approved =>
                new("approved", approved.ApplicationId, approved.IsReturningCustomer, null),
            SubmitLoanApplicationResult.Denied denied =>
                new("denied", null, null, [.. denied.Reasons.Select(reason => new DenialReasonResponse(reason.Code, reason.Message))]),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, "Unknown submission result."),
        };
}

public sealed record DenialReasonResponse(string Code, string Message);
