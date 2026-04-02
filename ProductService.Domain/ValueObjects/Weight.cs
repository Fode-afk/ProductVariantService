using migApp.Shared.Results;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class Weight : ValueObject
{
    public double Value { get; }
    public WeightUnit Unit { get; }

    private Weight(double value, WeightUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static IResult<Weight> Create(double value, WeightUnit unit)
    {
        if (value <= 0)
            return Fail<Weight>(WeightErrors.Invalid());

        return Ok(new Weight(value, unit));
    }

    public double ToKg()
        => Value * Unit.ToKgFactor;

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
        yield return Unit;
    }

    public override string ToString()
        => $"{Value} {Unit}";

    public static implicit operator string(Weight weight) => weight.ToString();
}
