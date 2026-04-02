using MediatR;

namespace ProductService.Domain.Primitives;

public interface IPreCommitDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
