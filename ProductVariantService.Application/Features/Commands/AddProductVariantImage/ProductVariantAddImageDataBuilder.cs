using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using ProductVariantService.Domain.RequestData;
using ProductVariantService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.AddProductVariantImage;

internal static class ProductVariantAddImageDataBuilder
{
    public static IResult<ProductVariantAddImageData> Build(AddProductVariantImageCommand request)
    {
        var errors = new List<Error>();

        var imageUrlResult = ImageUrl.Create(request.ImageUrl);
        if (imageUrlResult.IsFailure)
            errors.Add(imageUrlResult.Error);

        var altTextResult = AltText.Create(request.AltText);
        if (altTextResult.IsFailure)
            errors.Add(altTextResult.Error);

        if (errors.Count > 0)
            return Fail<ProductVariantAddImageData>(
                Error.Validation(
                    CommonErrorCodes.ValidationFailed,
                    new Dictionary<string, object>
                    {
                        ["Errors"] = errors.Select(e => e.Code).ToList()
                    }));

        return Ok(new ProductVariantAddImageData(
            imageUrlResult.Value,
            altTextResult.Value));
    }
}