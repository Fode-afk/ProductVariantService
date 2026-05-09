using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.RequestData;
using ProductVariantService.Domain.Snapshots;
using ProductVariantService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.CreateProductVariant;

internal static class ProductVariantCreationDataBuilder
{
    public static IResult<ProductVariantCreationData> Build(
        CreateProductVariantCommand request,
        List<CharacteristicSnapshot> characteristicSnapshots)
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

        var snapshotMap = characteristicSnapshots
            .ToDictionary(x => x.CharacteristicId);

        var attributes = new List<VariantAttribute>();

        foreach (var (characteristicId, value) in request.Attributes)
        {
            if (!snapshotMap.TryGetValue(characteristicId, out var snapshot))
            {
                errors.Add(CharacteristicSnapshotErrors.NotFound());
                continue;
            }

            AttributeGroupName? groupName = null;
            if (!string.IsNullOrWhiteSpace(snapshot.GroupName))
            {
                var groupResult = AttributeGroupName.Create(snapshot.GroupName);
                if (groupResult.IsFailure)
                {
                    errors.Add(groupResult.Error);
                    continue;
                }
                groupName = groupResult.Value;
            }

            var nameResult = AttributeName.Create(snapshot.Name);
            if (nameResult.IsFailure)
                errors.Add(nameResult.Error);

            var valueResult = AttributeValue.Create(value);
            if (valueResult.IsFailure)
                errors.Add(valueResult.Error);

            if (nameResult.IsSuccess && valueResult.IsSuccess)
            {
                var attrResult = VariantAttribute.Create(
                    characteristicId,
                    nameResult.Value,
                    valueResult.Value,
                    snapshot.CharType,
                    groupName);

                if (attrResult.IsFailure)
                {
                    errors.Add(attrResult.Error);
                    continue;
                }

                attributes.Add(attrResult.Value);
            }
        }

        if (errors.Count > 0)
            return Fail<ProductVariantCreationData>(
                Error.Validation(
                    CommonErrorCodes.ValidationFailed,
                    new Dictionary<string, object>
                    {
                        ["Errors"] = errors.Select(e => e.Code).ToList()
                    }));

        return Ok(new ProductVariantCreationData(
            skuResult.Value,
            dimensions!,
            weight!,
            barcodeResult.Value,
            attributes));
    }
}
