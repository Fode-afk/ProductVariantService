using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class ProductVariantImageErrors
{
    public static Error MaxImagesReached(int maxImages) => 
        Error.InvalidArgument(ProductVariantImageErrorCodes.MaxImagesReached,
            $"Maximum number of images ({maxImages}) reached.");

    public static Error AlreadyExists() =>
        Error.AlreadyExists(ProductVariantImageErrorCodes.AlreadyExists,
            "Image already exists.");

    public static Error NotFound() =>
        Error.NotFound(ProductVariantImageErrorCodes.NotFound,
            "Image not found.");

    public static Error InvalidImageCount() =>
        Error.InvalidArgument(ProductVariantImageErrorCodes.InvalidImageCount,
            "Invalid image count.");

    public static Error DuplicateImageIds() =>
        Error.InvalidArgument(ProductVariantImageErrorCodes.DuplicateImageIds,
            "Duplicate image IDs.");
}

public static class ProductVariantImageErrorCodes
{
    public const string MaxImagesReached = "ProductVariantImage.MaxImagesReached";
    public const string AlreadyExists = "ProductVariantImage.AlreadyExists";
    public const string NotFound = "ProductVariantImage.NotFound";
    public const string InvalidImageCount = "ProductVariantImage.InvalidImageCount";
    public const string DuplicateImageIds = "ProductVariantImage.DuplicateImageIds";
}