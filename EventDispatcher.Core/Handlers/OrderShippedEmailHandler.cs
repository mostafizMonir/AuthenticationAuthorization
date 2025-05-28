using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventDispatcher.Core.Abstractions;
using EventDispatcher.Core.Events;

namespace EventDispatcher.Core.Handlers;
public class OrderShippedEmailHandler:IDomainEventHandler<OrderShippedEvent>
{
    public async Task HandleAsync(OrderShippedEvent domainEvent)
    {
        
    }
}
