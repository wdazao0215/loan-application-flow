namespace LoanFlow.Application.ExternalSync;

public sealed record LoanApplicationApproved(Guid CustomerId, Guid ApplicationId, bool IsReturningCustomer);
