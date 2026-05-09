using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AttributeGroupNameErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(AttributeGroupNameErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(AttributeGroupNameErrorCodes.TooLong);
}

public static class AttributeGroupNameErrorCodes
{
    public const string NullOrEmpty = "AttributeGroupName.NullOrEmpty";
    public const string TooLong = "AttributeGroupName.TooLong";
}