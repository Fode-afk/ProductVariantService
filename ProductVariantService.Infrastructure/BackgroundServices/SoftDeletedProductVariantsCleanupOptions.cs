namespace ProductVariantService.Infrastructure.BackgroundServices;

public sealed class SoftDeletedProductVariantsCleanupOptions
{
    public const string SectionName = "SoftDeletedProductVariantsCleanup";

    public TimeSpan Interval { get; init; } = TimeSpan.FromHours(6);
    public TimeSpan RetentionPeriod { get; init; } = TimeSpan.FromDays(30);
    public int BatchSize { get; init; } = 500;
}
