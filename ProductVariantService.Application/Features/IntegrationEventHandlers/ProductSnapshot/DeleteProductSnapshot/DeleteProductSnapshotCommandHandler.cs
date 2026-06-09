using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.DeleteProductSnapshot;

public sealed class DeleteProductSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<DeleteProductSnapshotCommand>
{
    public async Task Handle(DeleteProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductSnapshots
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);
        if (snapshot is null)
            return;

        var variants = await context.ProductVariants
            .Where(v => v.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);

        foreach (var variant in variants)
            variant.ForceDelete(timeProvider.GetUtcNow());

        context.ProductSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}
