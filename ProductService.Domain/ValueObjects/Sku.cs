using migApp.Shared.Results;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed partial class Sku : ValueObject
{
    [GeneratedRegex(@"^[A-Z0-9\-]{3,50}$", RegexOptions.Compiled)]
    private static partial Regex SKURegex();

    public string Value { get; }

    private Sku(string value)
    {
        Value = value;
    }

    public static IResult<Sku> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Sku>(SkuErrors.NullOrEmpty());

        value = value.Trim().ToUpperInvariant();

        if (!SKURegex().IsMatch(value))
            return Fail<Sku>(SkuErrors.InvalidFormat());

        return Ok(new Sku(value));
    }

    public static IResult<Sku> Generate(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return Fail<Sku>(SkuErrors.NullOrEmpty());

        var random = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return Create($"{prefix}-{random}");
    }


    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Sku sku) => sku.ToString();
}
