using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class ProductErrors
{
    public static Error AttributesRequired() => Error.InvalidArgument(ProductErrorCodes.AttributesRequired);
    public static Error TagsRequired() => Error.InvalidArgument(ProductErrorCodes.TagsRequired);
    public static Error ImageNotFound() => Error.NotFound(ProductErrorCodes.ImageNotFound);
}

public static class ProductErrorCodes
{
    public const string AttributesRequired = "Product.AttributesRequired";
    public const string TagsRequired = "Product.TagsRequired";
    public const string ImageNotFound = "Product.ImageNotFound";
}