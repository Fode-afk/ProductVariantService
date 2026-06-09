using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AttributeValueErrors
{
    public static Error NullOrEmpty() => 
        Error.InvalidArgument(AttributeValueErrorCodes.NullOrEmpty,
            "Attribute value cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AttributeValueErrorCodes.TooLong,
            $"Attribute value is too long. Maximum length is {maxLength} characters.");
}

public static class AttributeValueErrorCodes
{
    public const string NullOrEmpty = "AttributeValue.NullOrEmpty";
    public const string TooLong = "AttributeValue.TooLong";
}
