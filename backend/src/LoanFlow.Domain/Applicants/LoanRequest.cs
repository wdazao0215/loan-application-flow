namespace LoanFlow.Domain.Applicants;

public sealed record LoanRequest
{
    public const int NameMaxLength = 100;
    public const int CompanyNameMaxLength = 150;

    public LoanRequest(
        string? firstName,
        string? lastName,
        string? companyName,
        Address address,
        Ssn ssn,
        LoanAmount requestedAmount)
    {
        FirstName = Guard.NotBlank(firstName, "First name", NameMaxLength);
        LastName = Guard.NotBlank(lastName, "Last name", NameMaxLength);
        CompanyName = Guard.NotBlank(companyName, "Company name", CompanyNameMaxLength);
        Address = address;
        Ssn = ssn;
        RequestedAmount = requestedAmount;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string CompanyName { get; }

    public Address Address { get; }

    public Ssn Ssn { get; }

    public LoanAmount RequestedAmount { get; }
}
