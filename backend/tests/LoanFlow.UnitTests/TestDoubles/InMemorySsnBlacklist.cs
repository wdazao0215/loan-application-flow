using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Decisions;

namespace LoanFlow.UnitTests.TestDoubles;

internal sealed class InMemorySsnBlacklist(params string[] blacklistedSsns) : ISsnBlacklist
{
    private readonly HashSet<Ssn> _ssns = blacklistedSsns.Select(ssn => new Ssn(ssn)).ToHashSet();

    public Task<bool> ContainsAsync(Ssn ssn, CancellationToken cancellationToken) =>
        Task.FromResult(_ssns.Contains(ssn));
}
