using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class WeightUnitErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(WeightUnitErrorCodes.NullOrEmpty,
            "Weight unit cannot be null or empty.");
    public static Error UnsupportedUnit() =>
        Error.InvalidArgument(WeightUnitErrorCodes.UnsupportedUnit,
            "Unsupported weight unit.");
}

public static class WeightUnitErrorCodes
{
    public const string NullOrEmpty = "WeightUnit.NullOrEmpty";
    public const string UnsupportedUnit = "WeightUnit.UnsupportedUnit";
}