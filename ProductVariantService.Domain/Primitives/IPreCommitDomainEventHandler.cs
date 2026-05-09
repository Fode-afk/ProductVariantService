using MediatR;

namespace ProductVariantService.Domain.Primitives;

public interface IPreCommitDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
