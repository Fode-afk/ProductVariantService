namespace ProductVariantService.Domain.Abstractions;

public interface IProductContext
{
    bool ProductCanEditContent { get; }
}
