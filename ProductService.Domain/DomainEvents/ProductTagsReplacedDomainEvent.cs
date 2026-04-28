
using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductTagsReplacedDomainEvent(
    Guid ProductId,
    Guid ProductCardId,
    IReadOnlyList<Tag> Tags,
    DateTimeOffset UpdatedAt) : IDomainEvent;