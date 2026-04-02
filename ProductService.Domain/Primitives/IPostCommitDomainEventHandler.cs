using MediatR;

namespace ProductService.Domain.Primitives;

public interface IPostCommitDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
