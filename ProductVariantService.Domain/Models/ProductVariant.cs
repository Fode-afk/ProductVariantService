using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.Specifications.ProductVariant;
using ProductVariantService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Models;

public sealed class ProductVariant : AggregateRoot
{
    private ProductVariant() : base(Guid.Empty) { }

    private ProductVariant(
        Guid id,
        Guid productId,
        Sku sku,
        Dimensions dimensions,
        Weight weight,
        Barcode barcode,
        List<VariantAttribute> attributes,
        DateTimeOffset createdAt) : base(id)
    {
        ProductId = productId;
        SKU = sku;
        Dimensions = dimensions;
        Weight = weight;
        Barcode = barcode;
        _attributes = [.. attributes];
        CreatedAt = createdAt;
    }

    public Guid ProductId { get; private set; }   
    public Sku SKU { get; private set; }
    public Dimensions Dimensions { get; private set; }
    public Weight Weight { get; private set; }

    public Barcode Barcode { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static int MaxAttributes => 30;
    private readonly List<VariantAttribute> _attributes = [];
    public IReadOnlyCollection<VariantAttribute> Attributes => _attributes;

    public static int MaxImages => 30;
    private readonly List<ProductVariantImage> _images = [];
    public IReadOnlyCollection<ProductVariantImage> Images => _images;
    public bool HasMainImage => _images.Any(x => x.IsMain);

    public static IResult<ProductVariant> Create(
        ProductVariantCreationContext ctx,
        Guid productId,
        Sku sku,
        Dimensions dimensions,
        Weight weight,
        Barcode barcode,
        List<VariantAttribute> attributes,
        DateTimeOffset now)
    {
        var result = ProductVariantCreationSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<ProductVariant>(result.Error);

        var productVariant = new ProductVariant(
            Guid.NewGuid(),
            productId,
            sku,
            dimensions,
            weight,
            barcode,
            attributes,
            now);

        productVariant.RaiseDomainEvent(new ProductVariantCreatedDomainEvent(
            productVariant.Id,
            productVariant.ProductId,
            productVariant.HasMainImage,
            productVariant._attributes));

        return Ok(productVariant);
    }

    public IResult UpdateInfo(
        ProductVariantUpdateInfoContext ctx,
        Sku sku,
        Dimensions dimensions,
        Weight weight,
        Barcode barcode,
        DateTimeOffset now)
    {
        if (SKU == sku &&
            Dimensions == dimensions &&
            Weight == weight &&
            Barcode == barcode)
            return Ok();

        var result = ProductVariantUpdateInfoSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        SKU = sku;
        Dimensions = dimensions;
        Weight = weight;
        Barcode = barcode;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductVariantUpdatedDomainEvent(
            Id,
            Dimensions,
            Weight,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult AddProductImage(
        ImageUrl url,
        AltText alt,
        bool isMain,
        DateTimeOffset now)
    {
        if (_images.Count >= 10)
            return Fail(ProductImageErrors.MaxImagesReached());

        var imageResult = ProductVariantImage.Create(Id, url, alt, _images.Count, isMain);

        if (imageResult.IsFailure)
            return imageResult;

        var image = imageResult.Value;

        if (isMain || _images.Count == 0)
        {
            foreach (var img in _images)
                img.SetAsMain(false);

            image.SetAsMain(true);
        }

        _images.Add(image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAddedDomainEvent(
            Id,
            ProductId,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult RemoveProductImage(
        ImageUrl url,
        DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);
        if (image is null)
            return Fail(ProductVariantErrors.ImageNotFound());

        bool wasMain = image.IsMain;
        _images.Remove(image);

        RecalculateImageOrder();

        if (wasMain && _images.Count > 0)
            _images[0].SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageRemovedDomainEvent(
            Id,
            ProductId,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult ChangeImageOrder(
        ImageUrl url,
        int newOrder,
        DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);
        if (image is null)
            return Fail(ProductVariantErrors.ImageNotFound());

        if (newOrder < 0 || newOrder >= _images.Count)
            return Fail(ProductImageErrors.InvalidSortOrder());

        _images.Remove(image);
        _images.Insert(newOrder, image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageOrderChangedDomainEvent(
            Id, 
            ProductId,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }

    private void RecalculateImageOrder()
    {
        for (int i = 0; i < _images.Count; i++)
            _images[i].ChangeOrder(i);
    }

    public IResult SetMainImage(
        ImageUrl url,
        DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);

        if (image is null)
            return Fail(ProductVariantErrors.ImageNotFound());

        foreach (var img in _images)
            img.SetAsMain(false);

        image.SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageSetMainDomainEvent(
            Id,
            ProductId,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult UpdateImageAlt(
        ImageUrl url,
        AltText alt,
        DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);

        if (image is null)
            return Fail(ProductVariantErrors.ImageNotFound());

        var result = image.UpdateAlt(alt);

        if (result.IsFailure)
            return result;

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAltUpdatedDomainEvent(
            Id,
            ProductId,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }
}