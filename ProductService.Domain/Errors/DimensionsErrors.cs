using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class DimensionsErrors
{
    public static Error Invalid() => Error.InvalidArgument(DimensionsErrorCodes.Invalid);
} 

public static class DimensionsErrorCodes
{
    public const string Invalid = "Dimensions.Invalid";
}