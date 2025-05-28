namespace EventDispatcher.Core.Abstractions;
public interface IDomainEventHandler<T> where T : IDomainEvent
{
}
