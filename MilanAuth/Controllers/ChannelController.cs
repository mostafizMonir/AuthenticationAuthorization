using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetChannel.Services;

namespace MilanAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChannelController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<ChannelController> _logger;

    public ChannelController(IMessageService messageService, ILogger<ChannelController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    [HttpPost("PublishMsg")]
    public async Task<IActionResult> PublishMsg()
    {
        try
        {
            await _messageService.PublishMessageAsync("hello world");
            return Ok(new { message = "Message published successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message");
            return StatusCode(500, new { error = "Failed to publish message" });
        }
    }
}
