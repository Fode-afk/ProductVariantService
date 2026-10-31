using migApp.Shared.Enums.Characteristics;

namespace ProductVariantService.Domain.Snapshots;

public sealed class CharacteristicSnapshot
{
    public Guid CharacteristicId { get; init; }

    public required string Name { get; init; }

    public AttributeCharType CharType { get; init; }

    public string? GroupName { get; init; }

    public bool IsUnifying { get; init; }

    public Guid CategoryId { get; init; }
}
