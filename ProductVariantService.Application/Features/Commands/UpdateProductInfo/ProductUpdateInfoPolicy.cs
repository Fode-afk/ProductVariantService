using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.UpdateProductInfo;

internal static class ProductUpdateInfoPolicy
{
    //public static IResult<ProductUpdateInfoData> Build(UpdateProductInfoCommand request)
    //{
    //    var nameResult = Name.Create(request.Name);
    //    if (nameResult.IsFailure)
    //        return Fail<ProductUpdateInfoData>(nameResult.Error);
    //
    //    var dimensionUnitResult = DimensionUnit.From(request.Dimensions.Unit);
    //    if (dimensionUnitResult.IsFailure)
    //        return Fail<ProductUpdateInfoData>(dimensionUnitResult.Error);
    //
    //    var dimensionsResult = Dimensions.Create(
    //        request.Dimensions.Length,
    //        request.Dimensions.Width,
    //        request.Dimensions.Height,
    //        dimensionUnitResult.Value);
    //    if (dimensionsResult.IsFailure)
    //        return Fail<ProductUpdateInfoData>(dimensionsResult.Error);
    //
    //    var weightUnitResult = WeightUnit.From(request.Weight.Unit);
    //    if (weightUnitResult.IsFailure)
    //        return Fail<ProductUpdateInfoData>(weightUnitResult.Error);
    //
    //    var weightResult = Weight.Create(request.Weight.Value, weightUnitResult.Value);
    //    if (weightResult.IsFailure)
    //        return Fail<ProductUpdateInfoData>(weightResult.Error);
    //
    //    return Ok(
    //        new ProductUpdateInfoData(
    //            nameResult.Value,
    //            dimensionsResult.Value,
    //            weightResult.Value));
    //}
}
