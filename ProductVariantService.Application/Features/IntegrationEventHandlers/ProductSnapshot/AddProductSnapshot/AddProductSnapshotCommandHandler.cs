using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.AddProductSnapshot;

public sealed class AddProductSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductSnapshotCommand>
{
    public async Task Handle(AddProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductSnapshots
            .AnyAsync(x => x.ProductId == request.ProductId, cancellationToken);

        if (exists)
            return;

        context.ProductSnapshots.Add(
            new Domain.Snapshots.ProductSnapshot
            {
                ProductId = request.ProductId,
                VendorId = request.VendorId,
                CategoryId = request.CategoryId,
                CanBeModified = request.CanBeModified,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
