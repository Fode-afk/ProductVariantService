using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Application.Interfaces.Metrics;

namespace ProductVariantService.Infrastructure.Observability;

public sealed class ActiveVariantsMetricCollector(
    IServiceScopeFactory scopeFactory,
    IProductVariantMetrics metrics,
    ILogger<ActiveVariantsMetricCollector> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider
                    .GetRequiredService<IAppDbContext>();

                var count = await context.ProductVariants.CountAsync(cancellationToken);

                metrics.SetActiveVariantsCount(count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to collect active variants count");
            }
        }
    }
}