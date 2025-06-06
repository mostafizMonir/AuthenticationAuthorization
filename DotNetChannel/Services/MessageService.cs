using System.Threading.Tasks;

namespace DotNetChannel.Services;

public interface IMessageService
{
    Task PublishMessageAsync(string message);
}

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
    }

    public async Task PublishMessageAsync(string message)
    {
        // Here we'll implement the actual message publishing logic
        _logger.LogInformation($"Publishing message: {message}");
        await Task.CompletedTask; // Placeholder for actual implementation
    }
}
