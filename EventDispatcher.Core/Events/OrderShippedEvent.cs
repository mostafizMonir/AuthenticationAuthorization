using EventDispatcher.Core.Abstractions;

namespace EventDispatcher.Core.Events;

public class OrderShippedEvent : IDomainEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; set; }
    public Guid OrderId { get; }

    public OrderShippedEvent(Guid orderId)
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        OrderId = orderId;
    }
}
