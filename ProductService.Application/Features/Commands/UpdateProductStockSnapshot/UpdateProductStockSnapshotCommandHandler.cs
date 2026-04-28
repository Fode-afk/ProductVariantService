using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.UpdateProductStockSnapshot;

public sealed class UpdateProductStockSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductStockSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductStockSnapshotCommand request, CancellationToken cancellationToken)
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

        var result = product.UpdateStockSnapshot(
            request.Status,
            request.AvailableQuantity,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;
        
        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
