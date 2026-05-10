using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed class DeleteVendorSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<DeleteVendorSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(DeleteVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (snapshot is null)
            return Ok();

        context.VendorSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
