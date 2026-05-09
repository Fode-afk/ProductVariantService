using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class WeightUnitErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(WeightUnitErrorCodes.NullOrEmpty);
    public static Error UnsupportedUnit() => Error.InvalidArgument(WeightUnitErrorCodes.UnsupportedUnit);
}

public static class WeightUnitErrorCodes
{
    public const string NullOrEmpty = "WeightUnit.NullOrEmpty";
    public const string UnsupportedUnit = "WeightUnit.UnsupportedUnit";
}