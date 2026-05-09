using MediatR;

namespace ProductVariantService.Domain.Primitives;

public interface IPostCommitDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
