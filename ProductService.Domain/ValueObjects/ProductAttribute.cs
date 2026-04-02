using migApp.Shared.Results;
using ProductService.Domain.Primitives;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.ValueObjects;

public sealed class ProductAttribute : ValueObject
{
    public AttributeName Name { get; }
    public AttributeValue Value { get; }

    private ProductAttribute(AttributeName name, AttributeValue value)
    {
        Name = name;
        Value = value;
    }

    public static IResult<ProductAttribute> Create(string name, string value)
    {
        var nameResult = AttributeName.Create(name);

        if (nameResult.IsFailure)
            return Fail<ProductAttribute>(nameResult.Error);

        var valueResult = AttributeValue.Create(value);

        if (valueResult.IsFailure)
            return Fail<ProductAttribute>(valueResult.Error);

        return Ok(new ProductAttribute(nameResult.Value, valueResult.Value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Value;
    }
}
