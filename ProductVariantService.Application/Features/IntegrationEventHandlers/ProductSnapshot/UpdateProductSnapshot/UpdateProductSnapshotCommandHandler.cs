using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

public sealed class UpdateProductSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductSnapshots
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken);
        if (snapshot is null)
            return Fail(ProductSnapshotErrors.NotFound());

        if (request.Version <= snapshot.Version)
            return Ok();

        snapshot.CategoryId = request.CategoryId;
        snapshot.CanBeModified = request.CanBeModified;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}