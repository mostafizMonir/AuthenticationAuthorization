using EventDispatcher.Core.Events;

namespace EventDispatcher.Core.Abstractions;
public interface IEventDispatcher<T> where T : IDomainEvent
{
    Task DispatchAsync(T domainEvent);
}
