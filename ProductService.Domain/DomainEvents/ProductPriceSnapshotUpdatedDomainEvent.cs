using migApp.Shared.Domain.ValueObjects;
using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductPriceSnapshotUpdatedDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    Money PriceSnapshot,
    Money? OldPriceSnapshot,
    DateTimeOffset PriceUpdatedAt,
    DateTimeOffset UpdatedAt) : IDomainEvent;