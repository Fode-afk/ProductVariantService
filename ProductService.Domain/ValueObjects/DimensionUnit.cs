using migApp.Shared.Results;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class DimensionUnit : ValueObject
{
    public static readonly DimensionUnit Cm = new("cm", 1.0);
    public static readonly DimensionUnit Mm = new("mm", 0.1);
    public static readonly DimensionUnit Inch = new("in", 2.54);

    private static readonly Dictionary<string, DimensionUnit> _units = new()
    {
        ["cm"] = Cm,
        ["mm"] = Mm,
        ["in"] = Inch
    };

    public string Code { get; }
    public double ToCmFactor { get; }

    private DimensionUnit(string code, double toCmFactor)
    {
        Code = code;
        ToCmFactor = toCmFactor;
    }

    public static IResult<DimensionUnit> From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Fail<DimensionUnit>(DimensionUnitErrors.NullOrEmpty());
         
        code = code.ToLowerInvariant();

        if (!_units.TryGetValue(code, out var unit))
            return Fail<DimensionUnit>(DimensionUnitErrors.Invalid());

        return Ok(unit);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return ToCmFactor;
    }

    public override string ToString() => Code;

    public static implicit operator string(DimensionUnit dimensionUnit) => dimensionUnit.ToString();
}
