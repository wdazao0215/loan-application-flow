using LoanFlow.Domain;
using LoanFlow.Domain.Applicants;

namespace LoanFlow.UnitTests.Domain.Applicants;

public class SsnTests
{
    [Theory]
    [InlineData("123-45-6789")]
    [InlineData("123456789")]
    [InlineData("  123-45-6789 ")]
    public void Normalizes_accepted_formats_to_nine_digits(string input)
    {
        new Ssn(input).Value.ShouldBe("123456789");
    }

    [Fact]
    public void Formatted_and_plain_inputs_are_the_same_ssn()
    {
        new Ssn("123-45-6789").ShouldBe(new Ssn("123456789"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    [InlineData("12-345-6789")]
    [InlineData("abc-de-fghi")]
    public void Rejects_anything_that_is_not_nine_digits(string? input)
    {
        Should.Throw<DomainValidationException>(() => new Ssn(input));
    }

    [Fact]
    public void Never_prints_the_full_number()
    {
        var ssn = new Ssn("123-45-6789");

        ssn.ToString().ShouldBe("***-**-6789");
        $"{ssn}".ShouldNotContain("12345");
    }
}
