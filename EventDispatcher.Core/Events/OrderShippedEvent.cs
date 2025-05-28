using EventDispatcher.Core.Abstractions;

namespace EventDispatcher.Core.Events;
public class OrderShippedEvent :IDomainEvent
{
    public Guid OrderId { get; set; }
    public OrderShippedEvent(Guid orderId)
    {
        OrderId = orderId;
        OccurredOn = DateTime.Now;
    }

    public DateTime OccurredOn { get; set; }
}
