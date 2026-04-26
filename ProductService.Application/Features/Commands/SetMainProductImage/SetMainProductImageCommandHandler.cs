using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.SetMainProductImage;

public sealed class SetMainProductImageCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<SetMainProductImageCommand, IResult>
{
    public async Task<IResult> Handle(SetMainProductImageCommand request, CancellationToken cancellationToken)
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

        var imageUrlResult = ImageUrl.Create(request.Url);
        if (imageUrlResult.IsFailure)
            return imageUrlResult;

        var result = product.SetMainImage(imageUrlResult.Value, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}