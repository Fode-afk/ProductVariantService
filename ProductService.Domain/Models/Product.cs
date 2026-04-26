using migApp.Shared.Results;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.Models;

public sealed class Product : AggregateRoot
{
    private Product() : base(Guid.Empty) { }

    private Product(
        Guid id,
        Guid productCardId,
        Sku sku,
        Name name,
        Dimensions dimensions,
        Weight weight,
        Barcode barcode,
        bool isDefault,
        List<ProductAttribute> attributes,
        List<Tag> tags,
        DateTimeOffset createdAt) : base(id)
    {
        ProductCardId = productCardId;
        SKU = sku;
        Name = name;
        Dimensions = dimensions;
        Weight = weight;
        Barcode = barcode;
        IsDefault = isDefault;
        _attributes = [.. attributes];
        _tags = [.. tags];
        CreatedAt = createdAt;
    }

    public Guid ProductCardId { get; private set; }   

    public Sku SKU { get; private set; }
    public Name Name { get; private set; }

    public Dimensions Dimensions { get; private set; }
    public Weight Weight { get; private set; }

    public Barcode Barcode { get; private set; }

    public bool IsDefault { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private List<ProductAttribute> _attributes = [];
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

    private readonly List<ProductImage> _images = [];
    public IReadOnlyCollection<ProductImage> Images => _images;

    private List<Tag> _tags = [];
    public IReadOnlyCollection<Tag> Tags => _tags;

    public static IResult<Product> Create(
        Guid vendorId,
        Guid productCardId,
        Sku sku,
        Name name,
        Dimensions dimensions,
        Weight weight,
        Barcode barcode,
        bool isDefault,
        List<ProductAttribute> attributes,
        List<Tag> tags,
        DateTimeOffset now)
    {
        if (attributes is null || attributes.Count == 0)
            return Fail<Product>(ProductErrors.AttributesRequired());

        if (tags is null || tags.Count == 0)
            return Fail<Product>(ProductErrors.TagsRequired());

        var product =
            new Product(
                Guid.NewGuid(),
                productCardId,
                sku,
                name,
                dimensions,
                weight,
                barcode,
                isDefault,
                attributes,
                tags,
                now);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product, vendorId));

        return Ok(product);
    }

    public IResult UpdateInfo(
        Name name,
        Dimensions dimensions,
        Weight weight,
        DateTimeOffset now)
    {
        if (Name == name &&
            Dimensions == dimensions &&
            Weight == weight)
            return Ok();

        Name = name;
        Dimensions = dimensions;
        Weight = weight;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductInfoUpdatedDomainEvent(Id, ProductCardId));

        return Ok();
    }

    public IResult MarkAsDefault(DateTimeOffset now)
    {
        if (IsDefault == true)
            return Ok();

        IsDefault = true;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductMarkAsDefaultDomainEvent(Id, ProductCardId));

        return Ok();
    }

    public IResult UnmarkAsDefault(DateTimeOffset now)
    {
        if (IsDefault == false)
            return Ok();

        IsDefault = false;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductUnmarkAsDefaultDomainEvent(Id, ProductCardId));

        return Ok();
    }

    public IResult ReplaceAttributes(
        List<ProductAttribute> attributes,
        DateTimeOffset now)
    {
        if (attributes is null || attributes.Count == 0)
            return Fail<Product>(ProductErrors.AttributesRequired());

        if (attributes.Count > 30)
            return Fail(ProductErrors.MaxAttributesReached());

        if (_attributes.SequenceEqual(attributes))
            return Ok();

        _attributes = [.. attributes];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductAttributesReplacedDomainEvent(Id, ProductCardId));

        return Ok();
    }

    public IResult ReplaceTags(
        List<Tag> tags,
        DateTimeOffset now)
    {
        if (tags is null || tags.Count == 0)
            return Fail<Product>(ProductErrors.TagsRequired());

        if (tags.Count > 50)
            return Fail(ProductErrors.MaxTagsReached());

        if (_tags.SequenceEqual(tags))
            return Ok();

        _tags = [.. tags];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductTagsReplacedDomainEvent(Id, ProductCardId));

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

        var imageResult = ProductImage.Create(Id, url, alt, _images.Count, isMain);

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

        RaiseDomainEvent(new ProductImageAddedDomainEvent(Id, ProductCardId));
        return Ok();
    }

    public IResult RemoveProductImage(ImageUrl url, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);
        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        bool wasMain = image.IsMain;
        _images.Remove(image);

        RecalculateImageOrder();

        if (wasMain && _images.Count > 0)
            _images[0].SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageRemovedDomainEvent(Id, ProductCardId));
        return Ok();
    }

    public IResult ChangeImageOrder(ImageUrl url, int newOrder, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);
        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        if (newOrder < 0 || newOrder >= _images.Count)
            return Fail(ProductImageErrors.InvalidSortOrder());

        _images.Remove(image);
        _images.Insert(newOrder, image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageOrderChangedDomainEvent(Id, ProductCardId));
        return Ok();
    }

    private void RecalculateImageOrder()
    {
        for (int i = 0; i < _images.Count; i++)
            _images[i].ChangeOrder(i);
    }

    public IResult SetMainImage(ImageUrl url, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);

        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        foreach (var img in _images)
            img.SetAsMain(false);

        image.SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageSetMainDomainEvent(Id, ProductCardId));

        return Ok();
    }

    public IResult UpdateImageAlt(ImageUrl url, AltText alt, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Url == url);

        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        var result = image.UpdateAlt(alt);

        if (result.IsFailure)
            return result;

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAltUpdatedDomainEvent(Id, ProductCardId));

        return Ok();
    }
}
