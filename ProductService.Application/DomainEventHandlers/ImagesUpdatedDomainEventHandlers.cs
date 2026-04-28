using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Models;
using ProductService.Domain.Primitives;
using System.Text.Json;

namespace ProductService.Application.DomainEventHandlers;

public sealed class ImagesUpdatedDomainEventHandlers(IAppDbContext context) : 
    IPreCommitDomainEventHandler<ProductImageAddedDomainEvent>,
    IPreCommitDomainEventHandler<ProductImageAltUpdatedDomainEvent>,
    IPreCommitDomainEventHandler<ProductImageOrderChangedDomainEvent>,
    IPreCommitDomainEventHandler<ProductImageRemovedDomainEvent>,
    IPreCommitDomainEventHandler<ProductImageSetMainDomainEvent>
{
    public async Task Handle(ProductImageAddedDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductImageAltUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
         await UpdateImages(
            notification.ProductId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductImageOrderChangedDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductImageSetMainDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    private async Task UpdateImages(
        Guid productId,
        IReadOnlyList<ProductImage> images,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken = default)
    {
        var productReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

        if (productReadModel == null)
            return;

        var imagesConeverted = images
           .OrderBy(i => i.SortOrder)
           .Select(i => new
           {
               Url = i.Url.Value,
               Alt = i.Alt.Value,
               i.IsMain,
               i.SortOrder
           })
           .ToList();

        var mainImage = images
            .FirstOrDefault(i => i.IsMain)?.Url.Value;

        productReadModel.ImagesJson = JsonSerializer.Serialize(imagesConeverted);
        productReadModel.MainImage = mainImage ?? string.Empty;
        productReadModel.UpdatedAt = updatedAt;
    }
}
