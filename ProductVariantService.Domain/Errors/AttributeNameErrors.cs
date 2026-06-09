using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AttributeNameErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(AttributeNameErrorCodes.NullOrEmpty,
            "Attribute name cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AttributeNameErrorCodes.TooLong,
            $"Attribute name is too long. Maximum length is {maxLength} characters.");
}

public static class AttributeNameErrorCodes
{
    public const string NullOrEmpty = "AttributeName.NullOrEmpty";
    public const string TooLong = "AttributeName.TooLong";
}