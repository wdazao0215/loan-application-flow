using LoanFlow.Domain;
using LoanFlow.UnitTests.TestDoubles;

namespace LoanFlow.UnitTests.Domain.Applicants;

public class LoanRequestTests
{
    [Fact]
    public void Trims_the_applicant_names()
    {
        var request = LoanRequests.Valid(firstName: "  Ada ", companyName: " Engines LLC ");

        request.FirstName.ShouldBe("Ada");
        request.CompanyName.ShouldBe("Engines LLC");
    }

    [Fact]
    public void Requires_a_company_name()
    {
        Should.Throw<DomainValidationException>(() => LoanRequests.Valid(companyName: ""));
    }

    [Fact]
    public void Rejects_names_longer_than_the_limit()
    {
        Should.Throw<DomainValidationException>(() => LoanRequests.Valid(lastName: new string('a', 101)));
    }
}
