using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class DimensionUnitErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(DimensionUnitErrorCodes.NullOrEmpty,
            "Dimension unit cannot be null or empty.");

    public static Error Invalid() =>
        Error.InvalidArgument(DimensionUnitErrorCodes.Invalid,
            "Dimension unit is invalid.");
}

public static class DimensionUnitErrorCodes
{
    public const string NullOrEmpty = "DimensionUnit.NullOrEmpty";
    public const string Invalid = "DimensionUnit.Invalid";
}