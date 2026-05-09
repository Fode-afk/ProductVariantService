using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.AddVendorSnapshot;

public sealed class AddVendorSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddVendorSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        //var exists = await context.VendorSnapshots
        //    .AnyAsync(x => x.VendorId == request.VendorId, cancellationToken);
        //
        //if (exists)
        //    return Fail(VendorSnapshotErrors.AlreadyExists());
        //
        //context.VendorSnapshots.Add(
        //    new VendorSnapshot
        //    {        
        //        VendorId = request.VendorId,
        //        Status = request.Status,
        //        IsVerified = request.IsVerified,
        //        UpdatedAt = timeProvider.GetUtcNow()
        //    });
        //
        //await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
