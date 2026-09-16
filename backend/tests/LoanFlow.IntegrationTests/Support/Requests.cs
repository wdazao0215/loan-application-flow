using LoanFlow.Api.Contracts;

namespace LoanFlow.IntegrationTests.Support;

public static class Requests
{
    public const string ApprovableSsn = "521-45-7788";
    public const string BlacklistedSsn = "123-45-6789";

    public static SubmitLoanApplicationRequest LoanApplication(
        string ssn = ApprovableSsn,
        string state = "TX",
        decimal requestedAmount = 25_000m,
        string lastName = "Lovelace") =>
        new(
            "Ada",
            lastName,
            "Analytical Engines LLC",
            new AddressRequest("100 Congress Ave", "Austin", state, "78701"),
            ssn,
            requestedAmount);
}
