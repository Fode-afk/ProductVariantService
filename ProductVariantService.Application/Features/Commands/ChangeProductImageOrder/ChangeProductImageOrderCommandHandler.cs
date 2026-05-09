using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Messaging.IntegrationEvents.ProductCards;
using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.ChangeProductImageOrder;

public sealed class ChangeProductImageOrderCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<ChangeProductImageOrderCommand, IResult>
{
    public async Task<IResult> Handle(ChangeProductImageOrderCommand request, CancellationToken cancellationToken)
    {
        //var vendorResult = await vendorValidator.ValidateForModificationAsync(
        //    request.VendorId,
        //    cancellationToken);
        //if (vendorResult.IsFailure)
        //    return vendorResult;
        //
        //var product = await context.Products
        //    .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        //if (product == null)
        //    return Fail(ProductErrors.NotFound());
        //
        //var cardResult = await productCardValidator.ValidateForModification(
        //    product.ProductCardId,
        //    request.VendorId,
        //    cancellationToken);
        //if (cardResult.IsFailure)
        //    return cardResult;
        //
        //var imageUrlResult = ImageUrl.Create(request.Url);
        //if (imageUrlResult.IsFailure)
        //    return imageUrlResult;
        //
        //var result = product.ChangeImageOrder(
        //    imageUrlResult.Value,
        //    request.NewOrder,
        //    timeProvider.GetUtcNow());
        //if (result.IsFailure)
        //    return result;
        //
        //await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
