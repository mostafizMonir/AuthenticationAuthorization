using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace DotNetChannel.Services;

public class MessageReceiverService : BackgroundService
{
    private readonly ILogger<MessageReceiverService> _logger;
    private readonly IMessageService _messageService;

    public MessageReceiverService(ILogger<MessageReceiverService> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = _messageService.GetChannelReader();

        try
        {
            await foreach (var message in reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Received message: {message}");
                // Process the message here
                await Task.Delay(100, stoppingToken); // Simulate some processing time
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing messages");
        }
    }
}
