namespace EventDispatcher.Core.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
} 