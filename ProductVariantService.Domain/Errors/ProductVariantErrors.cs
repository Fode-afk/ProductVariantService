using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class ProductVariantErrors
{
    public static Error AttributesRequired() =>
        Error.InvalidArgument(ProductVariantErrorCodes.AttributesRequired,
            "Attributes are required.");

    public static Error ImageNotFound() =>
        Error.NotFound(ProductVariantErrorCodes.ImageNotFound,
            "Image not found.");

    public static Error NotFound() =>
        Error.NotFound(ProductVariantErrorCodes.NotFound,
            "Product variant not found.");

    public static Error MaxAttributesReached(int maxAttributes) =>
        Error.InvalidArgument(ProductVariantErrorCodes.MaxAttributesReached,
            $"Maximum number of attributes ({maxAttributes}) reached.");

    public static Error DuplicateCombination() =>
        Error.InvalidArgument(ProductVariantErrorCodes.DuplicateCombination,
            "Duplicate attribute combination.");

    public static Error AlreadyExists() =>
        Error.AlreadyExists(ProductVariantErrorCodes.AlreadyExists,
            "Product variant already exists.");

    public static Error DuplicateSkuOrBarcode() =>
        Error.InvalidArgument(ProductVariantErrorCodes.DuplicateSkuOrBarcode,
            "Duplicate SKU or barcode.");
}

public static class ProductVariantErrorCodes
{
    public const string AttributesRequired = "ProductVariant.AttributesRequired";
    public const string ImageNotFound = "ProductVariant.ImageNotFound";
    public const string NotFound = "ProductVariant.NotFound";
    public const string MaxAttributesReached = "ProductVariant.MaxAttributesReached";
    public const string InvalidId = "ProductVariant.InvalidId";
    public const string DuplicateCombination = "ProductVariant.DuplicateCombination";
    public const string AlreadyExists = "ProductVariant.AlreadyExists";
    public const string DuplicateSkuOrBarcode = "ProductVariant.DuplicateSkuOrBarcode";
}