using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class Dimensions : ValueObject
{
    public const double MaxDimension = 9999.99;

    public double Length { get; }
    public double Width { get; }
    public double Height { get; }
    public DimensionUnit Unit { get; }

    private Dimensions(
        double length,
        double width, 
        double height,
        DimensionUnit unit)
    {
        Length = length;
        Width = width;
        Height = height;
        Unit = unit;
    }

    public static IResult<Dimensions> Create(
        double length,
        double width,
        double height,
        DimensionUnit unit)
    {
        if (length <= 0 || width <= 0 || height <= 0)
            return Fail<Dimensions>(DimensionsErrors.Invalid());

        if (length > MaxDimension || width > MaxDimension || height > MaxDimension)
            return Fail<Dimensions>(DimensionsErrors.Invalid());

        return Ok(
            new Dimensions(
                length,
                width,
                height,
                unit));
    }

    public double Volume()
        => Length * Width * Height;

    public double VolumeInCm3()
        => Volume() * Unit.ToCmFactor * Unit.ToCmFactor * Unit.ToCmFactor;

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Length;
        yield return Width;
        yield return Height;
        yield return Unit;
    }

    public override string ToString()
        => $"{Length}x{Width}x{Height} {Unit}";

    public static implicit operator string(Dimensions dimensions) => dimensions.ToString();
}
