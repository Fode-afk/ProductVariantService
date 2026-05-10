using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.RequestData;
using ProductVariantService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.UpdateProductVariantInfo;

internal static class ProductVariantUpdateInfoDataBuilder
{
    public static IResult<ProductVariantUpdateInfoData> Build(UpdateProductVariantInfoCommand request)
    {
        var errors = new List<Error>();

        var skuResult = Sku.Create(request.SKU);
        if (skuResult.IsFailure)
            errors.Add(skuResult.Error);

        var dimensionUnitResult = DimensionUnit.From(request.Dimensions.Unit);
        if (dimensionUnitResult.IsFailure)
            errors.Add(dimensionUnitResult.Error);

        Dimensions? dimensions = null;
        if (dimensionUnitResult.IsSuccess)
        {
            var dimensionsResult = Dimensions.Create(
                request.Dimensions.Length,
                request.Dimensions.Width,
                request.Dimensions.Height,
                dimensionUnitResult.Value);
            if (dimensionsResult.IsFailure)
                errors.Add(dimensionsResult.Error);
            else
                dimensions = dimensionsResult.Value;
        }

        var weightUnitResult = WeightUnit.From(request.Weight.Unit);
        if (weightUnitResult.IsFailure)
            errors.Add(weightUnitResult.Error);

        Weight? weight = null;
        if (weightUnitResult.IsSuccess)
        {
            var weightResult = Weight.Create(request.Weight.Value, weightUnitResult.Value);
            if (weightResult.IsFailure)
                errors.Add(weightResult.Error);
            else
                weight = weightResult.Value;
        }

        var barcodeResult = Barcode.Create(request.Barcode);
        if (barcodeResult.IsFailure)
            errors.Add(barcodeResult.Error);

        if (errors.Count > 0)
            return Fail<ProductVariantUpdateInfoData>(
                Error.Validation(
                    CommonErrorCodes.ValidationFailed,
                    new Dictionary<string, object>
                    {
                        ["Errors"] = errors.Select(e => e.Code).ToList()
                    }));

        return Ok(new ProductVariantUpdateInfoData(
            skuResult.Value,
            dimensions!,
            weight!,
            barcodeResult.Value));
    }
}
