using System.Text.RegularExpressions;

namespace LoanFlow.Domain.Applicants;

public sealed partial record Address
{
    public const int StreetMaxLength = 200;
    public const int CityMaxLength = 100;

    public Address(string? street, string? city, string? state, string? zipCode)
    {
        Street = Guard.NotBlank(street, "Street", StreetMaxLength);
        City = Guard.NotBlank(city, "City", CityMaxLength);
        State = NormalizeState(state);
        ZipCode = NormalizeZipCode(zipCode);
    }

    public string Street { get; }

    public string City { get; }

    public string State { get; }

    public string ZipCode { get; }

    private static string NormalizeState(string? state)
    {
        var code = state?.Trim().ToUpperInvariant() ?? string.Empty;

        return UsStates.Contains(code)
            ? code
            : throw new DomainValidationException("State must be a two-letter US state code.");
    }

    private static string NormalizeZipCode(string? zipCode)
    {
        var trimmed = zipCode?.Trim() ?? string.Empty;

        return ZipCodeFormat().IsMatch(trimmed)
            ? trimmed
            : throw new DomainValidationException("ZIP code must be formatted as 12345 or 12345-6789.");
    }

    [GeneratedRegex(@"^\d{5}(-\d{4})?$")]
    private static partial Regex ZipCodeFormat();
}
