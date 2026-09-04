using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class WeightErrors
{
    public static Error Invalid() =>
        Error.InvalidArgument(WeightErrorCodes.Invalid,
            "Invalid weight.");
}

public static class WeightErrorCodes
{
    public const string Invalid = "Weight.Invalid";
}
