using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class ProductVariantErrors
{
    public static Error AttributesRequired() => Error.InvalidArgument(ProductVariantErrorCodes.AttributesRequired);
    public static Error ImageNotFound() => Error.NotFound(ProductVariantErrorCodes.ImageNotFound);
    public static Error DuplicateSku() => Error.AlreadyExists(ProductVariantErrorCodes.DuplicateSku);
    public static Error NotFound() => Error.NotFound(ProductVariantErrorCodes.NotFound);
    public static Error DefaultProductAlreadyExists() =>
        Error.AlreadyExists(ProductVariantErrorCodes.DefaultProductAlreadyExists);
    public static Error MaxAttributesReached() => Error.InvalidArgument(ProductVariantErrorCodes.MaxAttributesReached);
    public static Error InvalidPrice() => Error.InvalidArgument(ProductVariantErrorCodes.InvalidPrice);
    public static Error InvalidOldPrice() => Error.InvalidArgument(ProductVariantErrorCodes.InvalidOldPrice);
    public static Error InvalidAvailableQuantity() =>
        Error.InvalidArgument(ProductVariantErrorCodes.InvalidAvailableQuantity);
    public static Error DuplicateCombination() => Error.InvalidArgument(ProductVariantErrorCodes.DuplicateCombination);
}

public static class ProductVariantErrorCodes
{
    public const string AttributesRequired = "ProductVariant.AttributesRequired";
    public const string ImageNotFound = "ProductVariant.ImageNotFound";
    public const string DuplicateSku = "ProductVariant.DuplicateSku";
    public const string NotFound = "ProductVariant.NotFound";
    public const string DefaultProductAlreadyExists = "ProductVariant.DefaultProductAlreadyExists";
    public const string MaxAttributesReached = "ProductVariant.MaxAttributesReached";
    public const string InvalidPrice = "ProductVariant.InvalidPrice";
    public const string InvalidOldPrice = "ProductVariant.InvalidOldPrice";
    public const string InvalidAvailableQuantity = "ProductVariant.InvalidAvailableQuantity";
    public const string InvalidId = "ProductVariant.InvalidId";
    public const string DuplicateCombination = "ProductVariant.DuplicateCombination";
}