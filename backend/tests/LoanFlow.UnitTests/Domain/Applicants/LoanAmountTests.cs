using LoanFlow.Domain;
using LoanFlow.Domain.Applicants;

namespace LoanFlow.UnitTests.Domain.Applicants;

public class LoanAmountTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.005)]
    [InlineData(1_000_000_000.01)]
    public void Rejects_amounts_that_cannot_be_lent(decimal amount)
    {
        Should.Throw<DomainValidationException>(() => new LoanAmount(amount));
    }

    [Fact]
    public void Accepts_a_positive_amount_with_cents()
    {
        new LoanAmount(15_000.50m).Value.ShouldBe(15_000.50m);
    }
}
