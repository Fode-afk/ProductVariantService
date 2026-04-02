using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class WeightErrors
{
    public static Error Invalid() => Error.InvalidArgument(WeightErrorCodes.Invalid);
}

public static class WeightErrorCodes
{
    public const string Invalid = "Weight.Invalid";
}
