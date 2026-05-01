using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.UpdateProductPriceSnapshot;

internal sealed class UpdateProductPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductPriceSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductPriceSnapshotCommand request, CancellationToken cancellationToken)
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

        var priceResult = Money.Create(request.Price, Currency.USD);
        if (priceResult.IsFailure)
            return priceResult;

        Money? oldPrice = null;
        if (request.OldPrice != null)
        {
            var oldPriceResult = Money.Create(request.OldPrice.Value, Currency.USD);
            if (oldPriceResult.IsFailure)
                return oldPriceResult;

            oldPrice = oldPriceResult.Value;
        }

        var result = product.UpdatePriceSnapshot(priceResult.Value, oldPrice, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
