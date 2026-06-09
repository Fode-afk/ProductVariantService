using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AttributeGroupNameErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(AttributeGroupNameErrorCodes.NullOrEmpty,
            "Attribute group name cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AttributeGroupNameErrorCodes.TooLong,
            $"Attribute group name is too long. Maximum length is {maxLength} characters.");
}

public static class AttributeGroupNameErrorCodes
{
    public const string NullOrEmpty = "AttributeGroupName.NullOrEmpty";
    public const string TooLong = "AttributeGroupName.TooLong";
}