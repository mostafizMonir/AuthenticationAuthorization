using EventDispatcher.Core.Abstractions;
using EventDispatcher.Core.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MilanAuth.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IEventDispatcher eventDispatcher) : ControllerBase
{
    public readonly IEventDispatcher EventDispatcher = eventDispatcher;

    [HttpPost]
    public async Task<IActionResult> ShipOrder()
    {
        var orderId = Guid.NewGuid(); // Simulate an order ID for demonstration purposes
        var orderShippedEvent = new OrderShippedEvent(orderId); // Assuming OrderShippedEvent is defined elsewhere
        await EventDispatcher.DispatchAsync(orderShippedEvent); // Dispatch the event asynchronously
        return Accepted();
    }
}
