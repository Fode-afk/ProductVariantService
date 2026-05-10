using migApp.Shared.Enums.Characteristics;

namespace ProductVariantService.Domain.Snapshots;

public sealed class CharacteristicSnapshot
{
    public Guid CharacteristicId { get; set; }

    public required string Name { get; set; }

    public AttributeCharType CharType { get; set; }

    public string? GroupName { get; set; }

    public bool IsUnifying { get; set; }

    public Guid CategoryId { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public long Version { get; set; }
}
