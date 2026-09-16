using System.Collections.Frozen;
using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Decisions;
using Microsoft.Extensions.Options;

namespace LoanFlow.Infrastructure.Blacklist;

internal sealed class ConfiguredSsnBlacklist(IOptions<SsnBlacklistOptions> options) : ISsnBlacklist
{
    private readonly FrozenSet<Ssn> _blacklistedSsns = options.Value.Ssns.Select(ssn => new Ssn(ssn)).ToFrozenSet();

    public Task<bool> ContainsAsync(Ssn ssn, CancellationToken cancellationToken) =>
        Task.FromResult(_blacklistedSsns.Contains(ssn));
}
