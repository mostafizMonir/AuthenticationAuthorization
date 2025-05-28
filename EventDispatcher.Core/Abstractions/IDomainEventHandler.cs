namespace EventDispatcher.Core.Abstractions;
public interface IDomainEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent);
}
