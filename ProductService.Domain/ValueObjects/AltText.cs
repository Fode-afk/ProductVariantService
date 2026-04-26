using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class AltText : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    private AltText(string value)
    {
        Value = value;
    }

    public static IResult<AltText> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<AltText>(AltTextErrors.NullOrEmpty());

        if (value.Length > MaxLength)
            return Fail<AltText>(AltTextErrors.TooLong());

        return Ok(new AltText(value.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(AltText altText) => altText.ToString();
}
