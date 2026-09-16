using LoanFlow.Api.Contracts;
using LoanFlow.Application.SubmitLoanApplication;
using Microsoft.AspNetCore.Mvc;

namespace LoanFlow.Api.Controllers;

[ApiController]
[Route("api/loan-applications")]
public sealed class LoanApplicationsController(SubmitLoanApplicationHandler submitLoanApplication) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<LoanApplicationDecisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<LoanApplicationDecisionResponse> Submit(
        SubmitLoanApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await submitLoanApplication.HandleAsync(request.ToCommand(), cancellationToken);
        return LoanApplicationDecisionResponse.From(result);
    }
}
