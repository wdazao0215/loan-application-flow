using LoanFlow.Domain.Applicants;

namespace LoanFlow.Domain.Decisions;

public interface ISsnBlacklist
{
    Task<bool> ContainsAsync(Ssn ssn, CancellationToken cancellationToken);
}
