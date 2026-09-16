namespace LoanFlow.Application.Abstractions;

public sealed class ConcurrentSubmissionException(Exception innerException)
    : Exception("Another submission for the same applicant was saved at the same time.", innerException);
