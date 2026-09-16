using LoanFlow.Application.SubmitLoanApplication;

namespace LoanFlow.Api.Contracts;

public sealed record SubmitLoanApplicationRequest(
    string? FirstName,
    string? LastName,
    string? CompanyName,
    AddressRequest? Address,
    string? Ssn,
    decimal RequestedAmount)
{
    public SubmitLoanApplicationCommand ToCommand() =>
        new(
            FirstName,
            LastName,
            CompanyName,
            Address?.Street,
            Address?.City,
            Address?.State,
            Address?.ZipCode,
            Ssn,
            RequestedAmount);
}

public sealed record AddressRequest(string? Street, string? City, string? State, string? ZipCode);
