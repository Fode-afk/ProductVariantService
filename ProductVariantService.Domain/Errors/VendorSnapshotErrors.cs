using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class VendorSnapshotErrors
{
    public static Error NotFound() => 
        Error.NotFound(VendorSnapshotErrorCodes.NotFound,
            "Vendor snapshot not found.");

    public static Error CannotModify() =>
        Error.Unauthenticated(VendorSnapshotErrorCodes.CannotModify,
            "Cannot modify vendor snapshot.");
}

public static class VendorSnapshotErrorCodes
{
    public const string NotFound = "VendorSnapshot.NotFound";
    public const string CannotModify = "VendorSnapshot.CannotModify";
}
