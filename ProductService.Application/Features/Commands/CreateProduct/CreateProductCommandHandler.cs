using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<CreateProductCommand, IResult>
{
    public async Task<IResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var card = await context.ProductCardSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductCardId == request.ProductCardId, cancellationToken);

        if (card is null)
            return Fail(ProductCardSnapshotErrors.NotFound());

        if (card.VendorId != request.VendorId)
            return Fail(ProductCardSnapshotErrors.InvalidVendor());

        var skuResult = Sku.Create(request.SKU);
        if (skuResult.IsFailure)
            return skuResult;

        var existsProductWithSku = await context.Products
            .AnyAsync(p => p.SKU == skuResult.Value, cancellationToken);
        if (existsProductWithSku)
            return Fail(ProductErrors.DuplicateSku());

        var existsDefaultProduct = await context.Products
            .AnyAsync(p =>
                p.ProductCardId == request.ProductCardId &&
                p.IsDefault,
                cancellationToken);
        if (existsDefaultProduct && request.IsDefault)
            return Fail(ProductErrors.DefaultProductAlreadyExists());

        var nameResult = Name.Create(request.Name);
        if (nameResult.IsFailure)
            return nameResult;

        var name = nameResult.Value;
        
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

        var barcodeResult = Barcode.Create(request.Barcode);
        if (barcodeResult.IsFailure)
            return barcodeResult;

        var attributeResults = request.Attributes
            .Select(a => ProductAttribute.Create(a.Key, a.Value))
            .ToList();

        var failed = attributeResults.FirstOrDefault(r => r.IsFailure);
        if (failed is not null)
            return failed;

        var tagResults = request.Tags
            .Select(Tag.Create)
            .ToList();

        var failedTag = tagResults.FirstOrDefault(r => r.IsFailure);
        if (failedTag is not null)
            return failedTag;

        var result = Product.Create(
            card.VendorId,
            request.ProductCardId, 
            skuResult.Value,
            name,
            dimensionsResult.Value,
            weightResult.Value,
            barcodeResult.Value,
            request.IsDefault,
            [.. attributeResults.Select(r => r.Value)],
            [.. tagResults.Select(r => r.Value)],
            timeProvider.GetUtcNow());

        if (result.IsFailure)
            return result;

        context.Products.Add(result.Value);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}