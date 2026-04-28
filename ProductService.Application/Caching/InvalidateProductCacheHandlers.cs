using ProductService.Domain.DomainEvents;
using ProductService.Domain.Primitives;
using ZiggyCreatures.Caching.Fusion;

namespace ProductService.Application.Caching;

public sealed class InvalidateProductCacheHandlers(IFusionCache cache) : 
    IPostCommitDomainEventHandler<ProductInfoUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<ProductMarkAsDefaultDomainEvent>,
    IPostCommitDomainEventHandler<ProductUnmarkAsDefaultDomainEvent>,
    IPostCommitDomainEventHandler<ProductAttributesReplacedDomainEvent>,
    IPostCommitDomainEventHandler<ProductTagsReplacedDomainEvent>,
    IPostCommitDomainEventHandler<ProductImageAddedDomainEvent>,
    IPostCommitDomainEventHandler<ProductImageRemovedDomainEvent>,
    IPostCommitDomainEventHandler<ProductImageOrderChangedDomainEvent>,
    IPostCommitDomainEventHandler<ProductImageAltUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<ProductImageSetMainDomainEvent>,
    IPostCommitDomainEventHandler<ProductPriceSnapshotUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<ProductStockSnapshotUpdatedDomainEvent>

{
    public async Task Handle(ProductInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) => 
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductMarkAsDefaultDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductUnmarkAsDefaultDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductAttributesReplacedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductTagsReplacedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductImageAddedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductImageOrderChangedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductImageAltUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductImageSetMainDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductPriceSnapshotUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductStockSnapshotUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    private async Task HandleInternal(Guid productCardId, CancellationToken cancellationToken) =>
        await cache.RemoveByTagAsync(CacheTags.ProductsByCardId(productCardId), token: cancellationToken);
}