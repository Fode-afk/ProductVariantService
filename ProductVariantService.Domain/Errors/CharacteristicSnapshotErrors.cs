using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class CharacteristicSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(CharacteristicSnapshotErrorCodes.NotFound);
    public static Error NotVariable() => Error.InvalidArgument(CharacteristicSnapshotErrorCodes.NotVariable);
    public static Error CannotUseUnifyingAsVariant() => Error.InvalidArgument(CharacteristicSnapshotErrorCodes.CannotUseUnifyingAsVariant);
}

public static class CharacteristicSnapshotErrorCodes
{
    public const string NotFound = "CharacteristicSnapshot.NotFound";
    public const string NotVariable = "CharacteristicSnapshot.NotVariable";
    public const string CannotUseUnifyingAsVariant = "CharacteristicSnapshot.CannotUseUnifyingAsVariant";
}