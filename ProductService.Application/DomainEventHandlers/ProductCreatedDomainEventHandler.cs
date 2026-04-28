using ProductService.Application.Interfaces.Data;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Models;
using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;
using System.Text.Json;

namespace ProductService.Application.DomainEventHandlers;

public sealed class ProductCreatedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCreatedDomainEvent>
{
    public Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var product = notification.Product;

        var attributesDict = product.Attributes
            .ToDictionary(a => a.Name.Value, a => a.Value.Value);

        var tags = product.Tags
            .Select(t => t.Value)
            .ToList();

        var images = product.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => new
            {
                Url = i.Url.Value,
                Alt = i.Alt.Value,
                i.IsMain,
                i.SortOrder
            })
            .ToList();

        var mainImage = product.Images
            .FirstOrDefault(i => i.IsMain)?.Url.Value;

        var readModel = new ProductReadModel
        {
            Id = product.Id,
            ProductCardId = product.ProductCardId,
            SKU = product.SKU.Value,
            Name = product.Name.Value,
            NameNormalized = Name.Normalize(product.Name),
            Barcode = product.Barcode.Value,
            Length = product.Dimensions.Length,
            Width = product.Dimensions.Width,
            Height = product.Dimensions.Height,
            DimensionUnit = product.Dimensions.Unit.Code,
            Weight = product.Weight.Value,
            WeightUnit = product.Weight.Unit.Code,
            MainImage = mainImage ?? string.Empty,
            AttributesJson = JsonSerializer.Serialize(attributesDict),
            TagsJson = JsonSerializer.Serialize(tags),
            TagsFlat = string.Join(",", tags),
            ImagesJson= JsonSerializer.Serialize(images),
            IsDefault = product.IsDefault,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        context.ProductReadModels.Add(readModel);

        return Task.CompletedTask;
    }
}
