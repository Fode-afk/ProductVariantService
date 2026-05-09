namespace ProductVariantService.Domain.Abstractions;

public interface IProductContext
{
    bool ProductCanBeModified { get; }
}
