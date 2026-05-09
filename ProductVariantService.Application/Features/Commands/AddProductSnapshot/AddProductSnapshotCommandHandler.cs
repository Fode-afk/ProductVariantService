using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Snapshots;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.AddProductSnapshot;

public sealed class AddProductSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductSnapshots
            .AnyAsync(x => x.ProductId == request.ProductId, cancellationToken);

        if (exists)
            return Fail(ProductSnapshotErrors.AlreadyExists());

        context.ProductSnapshots.Add(
            new ProductSnapshot
            {
                ProductId = request.ProductId,
                VendorId = request.VendorId,
                CategoryId = request.CategoryId,
                CanBeModified = request.CanBeModified,
                UpdatedAt = timeProvider.GetUtcNow()
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
