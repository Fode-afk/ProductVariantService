using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class ProductErrors
{
    public static Error AttributesRequired() => Error.InvalidArgument(ProductErrorCodes.AttributesRequired);
    public static Error TagsRequired() => Error.InvalidArgument(ProductErrorCodes.TagsRequired);
    public static Error ImageNotFound() => Error.NotFound(ProductErrorCodes.ImageNotFound);
    public static Error DuplicateSku() => Error.AlreadyExists(ProductErrorCodes.DuplicateSku);
    public static Error NotFound() => Error.NotFound(ProductErrorCodes.NotFound);
    public static Error DefaultProductAlreadyExists() => Error.AlreadyExists(ProductErrorCodes.DefaultProductAlreadyExists);
    public static Error MaxAttributesReached() => Error.InvalidArgument(ProductErrorCodes.MaxAttributesReached);
    public static Error MaxTagsReached() => Error.InvalidArgument(ProductErrorCodes.MaxTagsReached);
}

public static class ProductErrorCodes
{
    public const string AttributesRequired = "Product.AttributesRequired";
    public const string TagsRequired = "Product.TagsRequired";
    public const string ImageNotFound = "Product.ImageNotFound";
    public const string DuplicateSku = "Product.DuplicateSku";
    public const string NotFound = "Product.NotFound";
    public const string DefaultProductAlreadyExists = "Product.DefaultProductAlreadyExists";
    public const string MaxAttributesReached = "Product.MaxAttributesReached";
    public const string MaxTagsReached = "Product.MaxTagsReached";
}