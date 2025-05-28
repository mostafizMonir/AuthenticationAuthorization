using EventDispatcher.Core.Events;

namespace EventDispatcher.Core.Abstractions;
public interface IEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent);
}
