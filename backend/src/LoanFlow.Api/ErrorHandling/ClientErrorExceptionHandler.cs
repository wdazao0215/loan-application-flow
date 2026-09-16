using LoanFlow.Application.Abstractions;
using LoanFlow.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LoanFlow.Api.ErrorHandling;

public sealed class ClientErrorExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            DomainValidationException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "The loan application is not valid.",
                Detail = exception.Message,
            },
            ConcurrentSubmissionException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "The loan application was submitted twice at the same time.",
                Detail = "Please submit it again.",
            },
            _ => null,
        };

        if (problem is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
        });
    }
}
