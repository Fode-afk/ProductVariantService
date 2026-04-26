using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.AddProductImage;

public sealed class AddProductImageCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductImageCommand, IResult>
{
    public async Task<IResult> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
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

        var imageUrlResult = ImageUrl.Create(request.ImageUrl);
        if (imageUrlResult.IsFailure)
            return imageUrlResult;

        var altTextResult = AltText.Create(request.AltText);
        if (altTextResult.IsFailure)
            return altTextResult;

        var result = product.AddProductImage(
            imageUrlResult.Value,
            altTextResult.Value,
            request.IsMain,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
