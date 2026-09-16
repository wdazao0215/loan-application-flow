using LoanFlow.Domain;
using LoanFlow.Domain.Applicants;

namespace LoanFlow.UnitTests.Domain.Applicants;

public class AddressTests
{
    [Fact]
    public void Normalizes_the_state_to_an_uppercase_code()
    {
        new Address("1 Main St", "Albany", " ny ", "12207").State.ShouldBe("NY");
    }

    [Theory]
    [InlineData("New York")]
    [InlineData("XX")]
    [InlineData("")]
    public void Rejects_states_that_are_not_us_state_codes(string state)
    {
        Should.Throw<DomainValidationException>(() => new Address("1 Main St", "Albany", state, "12207"));
    }

    [Theory]
    [InlineData("12207")]
    [InlineData("12207-1234")]
    public void Accepts_five_and_nine_digit_zip_codes(string zipCode)
    {
        new Address("1 Main St", "Albany", "NY", zipCode).ZipCode.ShouldBe(zipCode);
    }

    [Theory]
    [InlineData("1220")]
    [InlineData("ABCDE")]
    public void Rejects_malformed_zip_codes(string zipCode)
    {
        Should.Throw<DomainValidationException>(() => new Address("1 Main St", "Albany", "NY", zipCode));
    }

    [Fact]
    public void Requires_a_street()
    {
        Should.Throw<DomainValidationException>(() => new Address("  ", "Albany", "NY", "12207"));
    }
}
