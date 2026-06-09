using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class CharacteristicSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(CharacteristicSnapshotErrorCodes.NotFound,
            "Characteristic snapshot not found.");

    public static Error CannotUseUnifyingAsVariant() =>
        Error.InvalidArgument(CharacteristicSnapshotErrorCodes.CannotUseUnifyingAsVariant,
            "Cannot use unifying characteristic snapshot as variant.");
}

public static class CharacteristicSnapshotErrorCodes
{
    public const string NotFound = "CharacteristicSnapshot.NotFound";
    public const string CannotUseUnifyingAsVariant = "CharacteristicSnapshot.CannotUseUnifyingAsVariant";
}