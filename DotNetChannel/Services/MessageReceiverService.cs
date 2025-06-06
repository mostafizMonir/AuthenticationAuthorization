using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetChannel.Services;

public class MessageReceiverService : BackgroundService
{
    private readonly ILogger<MessageReceiverService> _logger;

    public MessageReceiverService(ILogger<MessageReceiverService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Message receiver service is running at: {time}", DateTimeOffset.Now);
            // Here we'll implement the actual message receiving logic
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
