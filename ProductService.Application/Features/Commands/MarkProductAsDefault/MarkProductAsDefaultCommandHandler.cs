using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.MarkProductAsDefault;

public sealed class MarkProductAsDefaultCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<MarkProductAsDefaultCommand, IResult>
{
    public async Task<IResult> Handle(MarkProductAsDefaultCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
           .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product == null)
            return Fail(ProductErrors.NotFound());

        var card = await context.ProductCardSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ProductCardId == product.ProductCardId, cancellationToken);

        if (card is null)
            return Fail(ProductCardSnapshotErrors.NotFound());

        if (card.VendorId != request.VendorId)
            return Fail(ProductCardSnapshotErrors.InvalidVendor());

        if (card.Status == ProductCardStatus.Archived)
            return Fail(ProductCardSnapshotErrors.CannotModifyWhenArchived());

        if (product.IsDefault)
            return Ok();

        var now = timeProvider.GetUtcNow();
    
        var currentDefault = await context.Products
            .Where(p =>
                p.ProductCardId == product.ProductCardId &&
                p.IsDefault)
            .FirstOrDefaultAsync(cancellationToken);
        
        var unmarkResult = currentDefault?.UnmarkAsDefault(now);
        if (unmarkResult?.IsFailure == true)
            return unmarkResult;

        var result = product.MarkAsDefault(now);
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
