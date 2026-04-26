using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class Tag : ValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private Tag(string value)
    {
        Value = value;
    }

    public static IResult<Tag> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Tag>(TagErrors.Empty());

        if (value.Length > MaxLength)
            return Fail<Tag>(TagErrors.TooLong());

        return Ok(new Tag(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
