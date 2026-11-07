using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.ValueObjects;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed partial class Barcode : ValueObject
{
    public string Value { get; }
    public BarcodeType Type { get; }

    private Barcode(string value, BarcodeType type)
    {
        Value = value;
        Type = type;
    }

    public static IResult<Barcode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Barcode>(BarcodeErrors.NullOrEmpty());

        value = NormalizeRegex().Replace(value, "");

        if (!IsValid(value, out var type))
            return Fail<Barcode>(BarcodeErrors.InvalidFormat());

        return Ok(new Barcode(value, type));
    }

    private static bool IsValid(string value, out BarcodeType type)
    {
        type = BarcodeType.Unknown;

        if (!ValidationRegex().IsMatch(value))
            return false;

        type = BarcodeType.FromLength(value.Length);
        return type != BarcodeType.Unknown;
    }

    [GeneratedRegex(@"[\s\-]")]
    private static partial Regex NormalizeRegex();

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex ValidationRegex();

    public override IEnumerable<object> GetAtomicValues() => [Value, Type];

    public override string ToString() => Value;
    public static implicit operator string(Barcode barcode) => barcode.ToString();
}
