namespace LoanFlow.Domain;

public sealed class DomainValidationException(string message) : Exception(message);
