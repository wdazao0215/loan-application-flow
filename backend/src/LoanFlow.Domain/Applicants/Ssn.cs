using System.Text.RegularExpressions;

namespace LoanFlow.Domain.Applicants;

public sealed partial record Ssn
{
    public Ssn(string? value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (!SsnFormat().IsMatch(trimmed))
        {
            throw new DomainValidationException("SSN must have 9 digits, formatted as 123-45-6789 or 123456789.");
        }

        Value = trimmed.Replace("-", string.Empty, StringComparison.Ordinal);
    }

    public string Value { get; }

    public string Masked() => $"***-**-{Value[^4..]}";

    public override string ToString() => Masked();

    [GeneratedRegex(@"^\d{3}-?\d{2}-?\d{4}$")]
    private static partial Regex SsnFormat();
}
