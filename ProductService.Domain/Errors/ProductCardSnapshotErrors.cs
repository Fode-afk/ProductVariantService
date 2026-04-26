using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class ProductCardSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(ProductCardSnapshotErrorCodes.NotFound);
    public static Error InvalidVendor() => Error.Unauthenticated(ProductCardSnapshotErrorCodes.InvalidVendor);
    public static Error AlreadyExists() => Error.AlreadyExists(ProductCardSnapshotErrorCodes.AlreadyExists);
    public static Error CannotModifyWhenPublished() => Error.InvalidArgument(ProductCardSnapshotErrorCodes.CannotModifyWhenPublished);
    public static Error CannotModifyWhenArchived() => Error.InvalidArgument(ProductCardSnapshotErrorCodes.CannotModifyWhenArchived);
}

public static class ProductCardSnapshotErrorCodes
{
    public const string NotFound = "ProductCardSnapshot.NotFound";
    public const string InvalidVendor = "ProductCardSnapshot.InvalidVendor";
    public const string AlreadyExists = "ProductCardSnapshot.AlreadyExists";
    public const string CannotModifyWhenPublished = "ProductCardSnapshot.CannotModifyWhenPublished";
    public const string CannotModifyWhenArchived = "ProductCardSnapshot.CannotModifyWhenArchived";
}