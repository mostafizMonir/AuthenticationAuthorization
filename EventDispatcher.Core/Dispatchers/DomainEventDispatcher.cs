using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventDispatcher.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Core.Dispatchers;
public class DomainEventDispatcher: IEventDispatcher<IDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent)
    {
        _logger.LogInformation("Dispatching event of type {EventType}", domainEvent.GetType().Name);
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        
        var handlers = _serviceProvider.GetServices(handlerType);
        

        foreach (var domainEventHandler in handlers)
        {
            _logger.LogInformation("Invoking handler {HandlerType} for event {EventType}", domainEventHandler.GetType().Name, domainEvent.GetType().Name);
            var method = handlerType.GetMethod("HandleAsync");
            if (method != null)
            {
                await ((Task) method.Invoke(domainEventHandler, new object[] { domainEvent }))!;
            }
        }
    }
}
