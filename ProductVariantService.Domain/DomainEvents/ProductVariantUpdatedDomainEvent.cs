using ProductVariantService.Domain.Primitives;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.DomainEvents;

public sealed record ProductVariantUpdatedDomainEvent(
    Guid ProductVariantId, 
    Dimensions Dimensions,
    Weight Weight,
    DateTimeOffset UpdatedAt) : IDomainEvent;