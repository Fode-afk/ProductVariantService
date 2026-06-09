using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed class DeleteVendorSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<DeleteVendorSnapshotCommand>
{
    public async Task Handle(DeleteVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (snapshot is null)
            return;

        context.VendorSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}
