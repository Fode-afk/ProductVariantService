using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.Models;

public sealed class ProductVariantImage : Entity
{
    private ProductVariantImage() : base(Guid.Empty) { }

    private ProductVariantImage(
        Guid id,
        Guid productVariantId,
        ImageUrl url,
        AltText alt,
        int sortOrder) : base(id)
    {
        ProductVariantId = productVariantId;
        Url = url;
        Alt = alt;
        SortOrder = sortOrder;
    }

    public Guid ProductVariantId { get; private set; }
    public ImageUrl Url { get; private set; }
    public AltText Alt { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsMain => SortOrder == 0;

    internal static ProductVariantImage Create(
        Guid productVariantId,
        ImageUrl url,
        AltText alt,
        int sortOrder)
    {
        return new ProductVariantImage(
            Guid.NewGuid(),
            productVariantId,
            url,
            alt,
            sortOrder);
    }

    internal void UpdateAlt(AltText alt) => Alt = alt;
    internal void ChangeOrder(int newOrder) => SortOrder = newOrder;
}