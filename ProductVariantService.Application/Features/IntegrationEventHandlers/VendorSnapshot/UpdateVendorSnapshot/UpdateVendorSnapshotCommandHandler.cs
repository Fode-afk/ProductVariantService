using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed class UpdateVendorSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateVendorSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (snapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

        if (request.Version <= snapshot.Version)
            return Ok();

        snapshot.IsActive = request.IsActive;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
