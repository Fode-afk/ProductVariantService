using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductVariantService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.ValueObjects;

public sealed class WeightUnit : ValueObject
{
    public static readonly WeightUnit Kilogram = new("kg", 1.0);
    public static readonly WeightUnit Gram = new("g", 0.001);
    public static readonly WeightUnit Pound = new("lb", 0.453592);

    private static readonly Dictionary<string, WeightUnit> _units = new()
    {
        ["kg"] = Kilogram,
        ["g"] = Gram,
        ["lb"] = Pound
    };

    public string Code { get; }
    public double ToKgFactor { get; }

    private WeightUnit(string code, double toKgFactor)
    {
        Code = code;
        ToKgFactor = toKgFactor;
    }

    public static IResult<WeightUnit> From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Fail<WeightUnit>(WeightUnitErrors.NullOrEmpty());

        code = code.ToLowerInvariant();

        if (!_units.TryGetValue(code, out var unit))
            return Fail<WeightUnit>(WeightUnitErrors.UnsupportedUnit());

        return Ok(unit);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return ToKgFactor;
    }

    public override string ToString() => Code;

    public static implicit operator string(WeightUnit weightUnit) => weightUnit.ToString();
}
