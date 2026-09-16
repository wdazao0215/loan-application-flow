namespace LoanFlow.Domain;

internal static class Guard
{
    public static string NotBlank(string? value, string fieldName, int maxLength)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            throw new DomainValidationException($"{fieldName} is required.");
        }

        if (trimmed.Length > maxLength)
        {
            throw new DomainValidationException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return trimmed;
    }
}
