namespace ProductVariantService.Domain.Abstractions;

internal interface IProductContext
{
    bool ProductCanEditContent { get; }
}
