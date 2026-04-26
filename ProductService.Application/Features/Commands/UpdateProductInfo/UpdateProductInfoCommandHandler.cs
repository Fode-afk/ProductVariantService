using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.UpdateProductInfo;

public sealed class UpdateProductInfoCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductInfoCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductInfoCommand request, CancellationToken cancellationToken)
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

        var nameResult = Name.Create(request.Name);
        if (nameResult.IsFailure)
            return nameResult;

        var dimensionUnitResult = DimensionUnit.From(request.Dimensions.Unit);
        if (dimensionUnitResult.IsFailure)
            return dimensionUnitResult;

        var dimensionsResult = Dimensions.Create(
            request.Dimensions.Length,
            request.Dimensions.Width,
            request.Dimensions.Height,
            dimensionUnitResult.Value);
        if (dimensionsResult.IsFailure)
            return dimensionsResult;

        var weightUnitResult = WeightUnit.From(request.Weight.Unit);
        if (weightUnitResult.IsFailure)
            return weightUnitResult;

        var weightResult = Weight.Create(request.Weight.Value, weightUnitResult.Value);
        if (weightResult.IsFailure)
            return weightResult;

        var result = product.UpdateInfo(
            nameResult.Value,
            dimensionsResult.Value,
            weightResult.Value,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
