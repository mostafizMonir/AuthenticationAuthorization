using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventDispatcher.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventDispatcher.Core.Dispatchers;
public class DomainEventDispatcher: IEventDispatcher<IDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        // var handlers  = AppDomain.CurrentDomain.GetAssemblies()
        //     .SelectMany(a => a.GetTypes())
        //     .Where(t => handlerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
        //     .Select(Activator.CreateInstance)
        //     .Cast<IDomainEventHandler<IDomainEvent>>()
        //     .ToList();
        var handlers = _serviceProvider.GetServices(handlerType);
        // var handlers = _serviceProvider.GetServices(handlerType)
        //     .Cast<IDomainEventHandler<IDomainEvent>>()
        //     .ToList();

        foreach (var domainEventHandler in handlers)
        {
            var method = handlerType.GetMethod("HandleAsync");
            if (method != null)
            {
                await ((Task) method.Invoke(domainEventHandler, new object[] { domainEvent }))!;
            }
        }
    }
}
