using ProductService.Domain.Enums;
using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductStockSnapshotUpdatedDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    StockStatus Status,
    int AvailableQuantitySnapshot,
    DateTimeOffset StockUpdatedAt,
    DateTimeOffset UpdatedAt) : IDomainEvent;