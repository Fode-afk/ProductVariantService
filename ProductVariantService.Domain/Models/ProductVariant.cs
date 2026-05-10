using migApp.Shared.Results;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.RequestData;
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

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public static int MaxAttributes => 30;
    private readonly List<VariantAttribute> _attributes = [];
    public IReadOnlyCollection<VariantAttribute> Attributes => _attributes;

    public static int MaxImages => 30;
    private readonly List<ProductVariantImage> _images = [];
    public IReadOnlyCollection<ProductVariantImage> Images => _images;
    public bool HasMainImage => _images.Count > 0;

    public static IResult<ProductVariant> Create(
        ProductVariantCreationContext ctx,
        ProductVariantCreationData data,
        Guid productId,
        DateTimeOffset now)
    {
        var result = ProductVariantCreationSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<ProductVariant>(result.Error);

        var productVariant = new ProductVariant(
            Guid.NewGuid(),
            productId,
            data.Sku,
            data.Dimensions,
            data.Weight,
            data.Barcode,
            data.Attributes,
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
        ProductVariantUpdateInfoData data,
        DateTimeOffset now)
    {
        if (SKU == data.Sku &&
            Dimensions == data.Dimensions &&
            Weight == data.Weight &&
            Barcode == data.Barcode)
            return Ok();

        var result = ProductVariantUpdateInfoSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        SKU = data.Sku;
        Dimensions = data.Dimensions;
        Weight = data.Weight;
        Barcode = data.Barcode;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductVariantUpdatedDomainEvent(
            Id,
            Dimensions,
            Weight,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult AddImage(
        ProductVariantAddImageContext ctx,
        ProductVariantAddImageData data,
        DateTimeOffset now)
    {
        var result = ProductVariantAddImageSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var alreadyExists = _images.Any(x => x.Url == data.Url);
        if (alreadyExists)
            return Fail(ProductVariantImageErrors.AlreadyExists());

        var image = ProductVariantImage.Create(
            Id,
            data.Url,
            data.Alt,
            _images.Count);

        _images.Add(image);
        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAddedDomainEvent(
            Id,
            HasMainImage,
            Version));

        return Ok();
    }

    public IResult RemoveImage(
        ProductVariantRemoveImageContext ctx,
        Guid imageId,
        DateTimeOffset now)
    {
        var result = ProductVariantRemoveImageSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null)
            return Fail(ProductVariantImageErrors.NotFound());

        _images.Remove(image);
        RecalculateImageOrder();
        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageRemovedDomainEvent(
            Id,
            HasMainImage,
            Version));

        return Ok();
    }

    private void RecalculateImageOrder()
    {
        var sorted = _images.OrderBy(x => x.SortOrder).ToList();
        for (int i = 0; i < sorted.Count; i++)
            sorted[i].ChangeOrder(i);
    }

    public IResult ReorderImages(
        ProductVariantReorderImagesContext ctx,
        List<Guid> imageIds,
        DateTimeOffset now)
    {
        var result = ProductVariantReorderImagesSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        if (imageIds.Count != _images.Count)
            return Fail(ProductVariantImageErrors.InvalidImageCount());

        if (imageIds.Distinct().Count() != imageIds.Count)
            return Fail(ProductVariantImageErrors.DuplicateImageIds());

        foreach (var id in imageIds)
            if (_images.All(x => x.Id != id))
                return Fail(ProductVariantImageErrors.NotFound());

        for (int i = 0; i < imageIds.Count; i++)
        {
            var image = _images.First(x => x.Id == imageIds[i]);
            image.ChangeOrder(i);
        }

        _images.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImagesReorderedDomainEvent(
            Id,
            ProductId,
            [.. Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult UpdateImageAlt(
        ProductVariantUpdateImageAltContext ctx,
        Guid imageId,
        AltText alt,
        DateTimeOffset now)
    {
        var result = ProductVariantUpdateImageAltSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return Fail(ProductVariantErrors.ImageNotFound());

        image.UpdateAlt(alt);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAltUpdatedDomainEvent(
            Id,
            ProductId,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult Delete(
        ProductVariantDeleteContext ctx,
        DateTimeOffset now)
    {
        if (IsDeleted)
            return Ok();

        var result = ProductVariantDeleteSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        IsDeleted = true;
        DeletedAt = now;

        RaiseDomainEvent(new ProductVariantDeletedDomainEvent(Id));

        return Ok();
    }

    public IResult ForceDelete(DateTimeOffset now)
    {
        if (IsDeleted)
            return Ok();

        IsDeleted = true;
        DeletedAt = now;

        RaiseDomainEvent(new ProductVariantForceDeletedDomainEvent(Id));

        return Ok();
    }
}