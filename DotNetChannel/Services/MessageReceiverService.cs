using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using DotNetChannel.Models;

namespace DotNetChannel.Services;

public class MessageReceiverService : BackgroundService
{
    private readonly ILogger<MessageReceiverService> _logger;
    private readonly IMessageService _messageService;
    private readonly IdGenerator _idGenerator;

    public MessageReceiverService(ILogger<MessageReceiverService> logger, IMessageService messageService, IdGenerator idGenerator)
    {
        _logger = logger;
        _messageService = messageService;
        _idGenerator = idGenerator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = _messageService.GetChannelReader();

        try
        {
            await foreach (var message in reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Received message: {message}");
                _logger.LogInformation($"id generator id : {_idGenerator.Id}");
                

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
