using DotNetChannel.Models;
using DotNetChannel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MilanAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChannelController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<ChannelController> _logger;
    private readonly IdPrinter IdPrinter;
    private readonly IdGenerator _idGenerator;


    public ChannelController(IMessageService messageService, ILogger<ChannelController> logger, IdPrinter idPrinter, IdGenerator idGenerator)
    {
        _messageService = messageService;
        _logger = logger;
        IdPrinter = idPrinter;
        _idGenerator = idGenerator;
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
    [HttpGet("GetId")]
    public IActionResult GetId()
    {
        try
        {
            var id = IdPrinter.PrintId();
            var idcl  = _idGenerator.Id; // Assuming you want to use the Id from IdGenerator as well
            return Ok(new  { printer = id,generator = idcl  });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ID");
            return StatusCode(500, new { error = "Failed to generate ID" });
        }
    }
}
