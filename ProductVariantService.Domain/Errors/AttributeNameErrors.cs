using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AttributeNameErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(AttributeNameErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(AttributeNameErrorCodes.TooLong);
}

public static class AttributeNameErrorCodes
{
    public const string NullOrEmpty = "AttributeName.NullOrEmpty";
    public const string TooLong = "AttributeName.TooLong";
}