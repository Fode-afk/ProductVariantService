using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class DimensionUnitErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(DimensionUnitErrorCodes.NullOrEmpty);
    public static Error Invalid() => Error.InvalidArgument(DimensionUnitErrorCodes.Invalid);
}

public static class DimensionUnitErrorCodes
{
    public const string NullOrEmpty = "DimensionUnit.NullOrEmpty";
    public const string Invalid = "DimensionUnit.Invalid";
}