using migApp.Shared.Results;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class AttributeName : ValueObject
{
    private const int MaxLength = 100;

    public string Value { get; }

    private AttributeName(string value)
    {
        Value = value;
    }

    public static IResult<AttributeName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<AttributeName>(AttributeNameErrors.TooLong());

        if (value.Length > MaxLength)
            return Fail<AttributeName>(AttributeNameErrors.TooLong());

        return Ok(new AttributeName(value.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(AttributeName attributeName) => attributeName.ToString();
}
