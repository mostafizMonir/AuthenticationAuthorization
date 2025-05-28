using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventDispatcher.Core.Abstractions;

namespace EventDispatcher.Core.Dispatchers;
internal class DomainEventDispatcher: IEventDispatcher<IDomainEvent>
{
    public Task DispatchAsync(IDomainEvent domainEvent)
    {
        
    }
}
