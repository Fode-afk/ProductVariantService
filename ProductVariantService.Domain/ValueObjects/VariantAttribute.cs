using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductVariantService.Domain.Enums;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.ValueObjects;

public sealed class VariantAttribute : ValueObject
{
    public Guid CharacteristicId { get; }
    public AttributeName Name { get; }
    public AttributeValue Value { get; }
    public AttributeCharType CharType { get; }
    public AttributeGroupName? GroupName { get; }

    private VariantAttribute() { }

    private VariantAttribute(
        Guid characteristicId,
        AttributeName name,
        AttributeValue value,
        AttributeCharType charType,
        AttributeGroupName? groupName)
    {
        CharacteristicId = characteristicId;
        Name = name;
        Value = value;
        CharType = charType;
        GroupName = groupName;
    }

    public static IResult<VariantAttribute> Create(
        Guid characteristicId,
        AttributeName name,
        AttributeValue value,
        AttributeCharType charType,
        AttributeGroupName? groupName = null)
    {
        return Ok(new VariantAttribute(
            characteristicId,
            name,
            value,
            charType,
            groupName));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CharacteristicId;
        yield return Name;
        yield return Value;
        yield return CharType;
        yield return GroupName ?? string.Empty;
    }
}
