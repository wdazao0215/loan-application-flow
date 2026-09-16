namespace LoanFlow.Infrastructure.Blacklist;

internal sealed class SsnBlacklistOptions
{
    public const string SectionName = "SsnBlacklist";

    public IReadOnlyList<string> Ssns { get; init; } = [];
}
