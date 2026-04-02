using migApp.Shared.Results;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class Name : ValueObject
{
    private const int MaxLength = 40;
    private const int MinLength = 3;

    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static IResult<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Name>(NameErrors.NullOrEmpty());

        if (value.Length > MaxLength)
            return Fail<Name>(NameErrors.TooLong());

        if (value.Length < MinLength)
            return Fail<Name>(NameErrors.TooShort());

        return Ok(new Name(value.Trim()));
    }

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.ToString();
}
