using migApp.Shared.Results;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Models;

public sealed class ProductVariantImage : Entity
{
    private ProductVariantImage() : base(Guid.Empty) { }
    private ProductVariantImage(
        Guid id,
        Guid productId,
        ImageUrl url,
        AltText alt,
        int sortOrder,
        bool isMain) : base(id)
    {
        ProductVariantId = productId;
        Url = url;
        Alt = alt;
        SortOrder = sortOrder;
        IsMain = isMain;
    }

    public Guid ProductVariantId { get; private set; }

    public ImageUrl Url { get; private set; }
    public AltText Alt { get; private set; }

    public int SortOrder { get; private set; }
    public bool IsMain { get; private set; }

    internal static IResult<ProductVariantImage> Create(
        Guid productId,
        ImageUrl url,
        AltText alt,
        int sortOrder,
        bool isMain)
    {
        return Ok(
            new ProductVariantImage(
                Guid.NewGuid(),
                productId,
                url,
                alt,
                sortOrder,
                isMain));
    }

    internal IResult SetAsMain(bool isMain)
    {
        if (IsMain == isMain)
            return Ok();

        IsMain = isMain;
        return Ok();
    }

    internal IResult UpdateAlt(AltText alt)
    {
        if (Alt == alt)
            return Ok();

        Alt = alt;

        return Ok();
    }

    internal IResult ChangeOrder(int newOrder)
    {
        if (newOrder < 0)
            return Fail(ProductImageErrors.InvalidSortOrder());

        if (SortOrder == newOrder)
            return Ok();

        SortOrder = newOrder;

        return Ok();
    }
}
