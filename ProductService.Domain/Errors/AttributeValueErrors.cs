using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class AttributeValueErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(AttributeValueErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(AttributeValueErrorCodes.TooLong);
}

public static class AttributeValueErrorCodes
{
    public const string NullOrEmpty = "AttributeValue.NullOrEmpty";
    public const string TooLong = "AttributeValue.TooLong";
}
