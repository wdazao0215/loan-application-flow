using LoanFlow.Domain.Applicants;

namespace LoanFlow.UnitTests.TestDoubles;

internal static class LoanRequests
{
    public const string ApprovableSsn = "521-45-7788";

    public static LoanRequest Valid(
        string ssn = ApprovableSsn,
        string state = "TX",
        decimal requestedAmount = 25_000m,
        string firstName = "Ada",
        string lastName = "Lovelace",
        string companyName = "Analytical Engines LLC",
        string street = "100 Congress Ave",
        string city = "Austin") =>
        new(
            firstName,
            lastName,
            companyName,
            new Address(street, city, state, "78701"),
            new Ssn(ssn),
            new LoanAmount(requestedAmount));
}
