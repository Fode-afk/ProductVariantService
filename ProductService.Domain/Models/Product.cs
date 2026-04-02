using migApp.Shared.Results;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Errors;
using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Domain.Models;

public sealed class Product : AggregateRoot
{
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
        List<string> tags,
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
    public DateTimeOffset UpdatedAt { get; private set; }

    private List<ProductAttribute> _attributes = [];
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

    private readonly List<ProductImage> _images = [];
    public IReadOnlyCollection<ProductImage> Images => _images;

    private List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags;

    public static IResult<Product> Create(
        Guid productCardId,
        Sku sku,
        Name name,
        Dimensions dimensions,
        Weight weight,
        Barcode barcode,
        bool isDefault,
        List<ProductAttribute> attributes,
        List<string> tags,
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

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));

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

        RaiseDomainEvent(new ProductInfoUpdatedDomainEvent(Id));

        return Ok();
    }

    public IResult SetDefault(bool isDefault, DateTimeOffset now)
    {
        if (IsDefault == isDefault)
            return Ok();

        IsDefault = isDefault;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductSetDefaultDomainEvent(Id));

        return Ok();
    }

    public IResult ReplaceAttributes(
        List<ProductAttribute> attributes,
        DateTimeOffset now)
    {
        if (attributes is null || attributes.Count == 0)
            return Fail<Product>(ProductErrors.AttributesRequired());

        if (_attributes.SequenceEqual(attributes))
            return Ok();

        _attributes = [.. attributes];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductAttributesReplacedDomainEvent(Id));

        return Ok();
    }

    public IResult ReplaceTags(
        List<string> tags,
        DateTimeOffset now)
    {
        if (tags is null || tags.Count == 0)
            return Fail<Product>(ProductErrors.TagsRequired());

        if (_tags.SequenceEqual(tags))
            return Ok();

        _tags = [.. tags];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductTagsReplacedDomainEvent(Id));

        return Ok();
    }

    public IResult AddProductImage(
        ImageUrl url,
        AltText alt,
        bool isMain,
        DateTimeOffset now)
    {
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

        RaiseDomainEvent(new ProductImageAddedDomainEvent(Id));
        return Ok();
    }

    public IResult RemoveProductImage(Guid imageId, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        bool wasMain = image.IsMain;
        _images.Remove(image);

        RecalculateImageOrder();

        if (wasMain && _images.Count > 0)
            _images[0].SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageRemovedDomainEvent(Id));
        return Ok();
    }

    public IResult ChangeImageOrder(Guid imageId, int newOrder, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        if (newOrder < 0 || newOrder >= _images.Count)
            return Fail(ProductImageErrors.InvalidSortOrder());

        _images.Remove(image);
        _images.Insert(newOrder, image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageOrderChangedDomainEvent(Id));
        return Ok();
    }

    private void RecalculateImageOrder()
    {
        for (int i = 0; i < _images.Count; i++)
            _images[i].ChangeOrder(i);
    }

    public IResult SetMainImage(Guid imageId, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        foreach (var img in _images)
            img.SetAsMain(false);

        image.SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageSetMainDomainEvent(Id));

        return Ok();
    }

    public IResult UpdateImageAlt(Guid imageId, AltText alt, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return Fail(ProductErrors.ImageNotFound());

        var result = image.UpdateAlt(alt);

        if (result.IsFailure)
            return result;

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAltUpdatedDomainEvent(Id));

        return Ok();
    }
}
