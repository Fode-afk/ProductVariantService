using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.UnmarkProductAsDefault;

public sealed class UnmarkProductAsDefaultCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UnmarkProductAsDefaultCommand, IResult>
{
    public async Task<IResult> Handle(UnmarkProductAsDefaultCommand request, CancellationToken cancellationToken)
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

        switch (card.Status)
        {
            case ProductCardStatus.Published:          
                return Fail(ProductCardSnapshotErrors.CannotModifyWhenPublished());
            case ProductCardStatus.Archived:
                return Fail(ProductCardSnapshotErrors.CannotModifyWhenArchived());
            case ProductCardStatus.Draft:
                var result = product.UnmarkAsDefault(timeProvider.GetUtcNow());
                if (result.IsFailure)
                    return result;
                break;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
