using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class RatingSnapshotErrors
{
    public static Error OutOfRange() => Error.InvalidArgument(RatingSnapshotErrorCodes.OutOfRange);
    public static Error InvalidCount() => Error.InvalidArgument(RatingSnapshotErrorCodes.InvalidCount);
}

public static class RatingSnapshotErrorCodes
{
    public const string OutOfRange = "RatingSnapshot.OutOfRange";
    public const string InvalidCount = "RatingSnapshot.InvalidCount";
}
