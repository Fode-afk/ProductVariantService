using ProductService.Domain.Primitives;

namespace ProductService.Domain.ValueObjects;

public sealed class BarcodeType : ValueObject
{
    public string Value { get; }

    private BarcodeType(string value)
    {
        Value = value;
    }

    public static BarcodeType EAN8 => new("EAN-8");
    public static BarcodeType EAN13 => new("EAN-13");
    public static BarcodeType UPCA => new("UPC-A");
    public static BarcodeType GTIN14 => new("GTIN-14");
    public static BarcodeType Unknown => new("Unknown");

    public static BarcodeType FromLength(int length)
    {
        return length switch
        {
            8 => EAN8,
            12 => UPCA,
            13 => EAN13,
            14 => GTIN14,
            _ => Unknown
        };
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(BarcodeType barcodeType)
        => barcodeType.ToString();
}
