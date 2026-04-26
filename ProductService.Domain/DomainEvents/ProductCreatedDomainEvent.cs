using ProductService.Domain.Models;
using ProductService.Domain.Primitives;

namespace ProductService.Domain.DomainEvents;

public sealed record ProductCreatedDomainEvent(Product Product, Guid VendorId) : IDomainEvent;