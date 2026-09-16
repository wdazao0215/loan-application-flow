using System.ComponentModel.DataAnnotations;

namespace LoanFlow.Infrastructure.ExternalService;

internal sealed class ExternalServiceOptions
{
    public const string SectionName = "ExternalService";

    [Required]
    public Uri BaseUrl { get; init; } = null!;

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
}
