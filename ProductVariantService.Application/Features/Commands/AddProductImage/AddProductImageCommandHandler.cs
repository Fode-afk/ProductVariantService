using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.AddProductImage;

public sealed class AddProductImageCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductImageCommand, IResult>
{
    public async Task<IResult> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
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
        //var imageUrlResult = ImageUrl.Create(request.ImageUrl);
        //if (imageUrlResult.IsFailure)
        //    return imageUrlResult;
        //
        //var altTextResult = AltText.Create(request.AltText);
        //if (altTextResult.IsFailure)
        //    return altTextResult;
        //
        //var result = product.AddProductImage(
        //    imageUrlResult.Value,
        //    altTextResult.Value,
        //    request.IsMain,
        //    timeProvider.GetUtcNow());
        //if (result.IsFailure)
        //    return result;
        //
        //await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
