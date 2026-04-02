
using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductTagsReplacedDomainEvent(Guid ProductId) : IDomainEvent;