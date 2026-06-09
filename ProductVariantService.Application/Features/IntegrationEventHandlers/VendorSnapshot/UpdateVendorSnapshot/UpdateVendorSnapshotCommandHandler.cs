using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Application.Interfaces.Metrics;
using ProductVariantService.Domain.Exceptions;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed class UpdateVendorSnapshotCommandHandler(
    IAppDbContext context,
    IProductVariantMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateVendorSnapshotCommand>
{
    public async Task Handle(UpdateVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);

        if (snapshot is null)
        {
            metrics.RecordSnapshotNotFound("Vendor");
            throw new SnapshotNotFoundException("Vendor", request.VendorId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateVendorSnapshotCommand));
            return;
        }

        snapshot.IsActive = request.IsActive;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);
    }
}
