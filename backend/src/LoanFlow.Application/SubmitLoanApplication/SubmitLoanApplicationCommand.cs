using LoanFlow.Domain.Applicants;

namespace LoanFlow.Application.SubmitLoanApplication;

public sealed record SubmitLoanApplicationCommand(
    string? FirstName,
    string? LastName,
    string? CompanyName,
    string? Street,
    string? City,
    string? State,
    string? ZipCode,
    string? Ssn,
    decimal RequestedAmount)
{
    internal LoanRequest ToLoanRequest() =>
        new(
            FirstName,
            LastName,
            CompanyName,
            new Address(Street, City, State, ZipCode),
            new Ssn(Ssn),
            new LoanAmount(RequestedAmount));
}
