using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class DimensionsErrors
{
    public static Error Invalid() => 
        Error.InvalidArgument(DimensionsErrorCodes.Invalid,
            "Dimensions are invalid.");
} 

public static class DimensionsErrorCodes
{
    public const string Invalid = "Dimensions.Invalid";
}