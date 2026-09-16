namespace LoanFlow.Domain.Applicants;

public sealed record LoanAmount
{
    public const decimal MaxValue = 1_000_000_000m;

    public LoanAmount(decimal value)
    {
        if (value <= 0)
        {
            throw new DomainValidationException("Requested amount must be greater than zero.");
        }

        if (value > MaxValue)
        {
            throw new DomainValidationException($"Requested amount cannot exceed {MaxValue:N0}.");
        }

        if (decimal.Round(value, 2) != value)
        {
            throw new DomainValidationException("Requested amount cannot have more than two decimal places.");
        }

        Value = value;
    }

    public decimal Value { get; }
}
