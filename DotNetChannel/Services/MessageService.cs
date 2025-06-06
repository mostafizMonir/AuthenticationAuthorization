using System.Threading.Tasks;
using System.Threading.Channels;

namespace DotNetChannel.Services;

public interface IMessageService
{
    Task PublishMessageAsync(string message);
    ChannelReader<string> GetChannelReader();
}

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly Channel<string> _channel;

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions 
        { 
            SingleReader = false,
            SingleWriter = false
        });
    }

    public async Task PublishMessageAsync(string message)
    {
        _logger.LogInformation($"Publishing message: {message}");
        await _channel.Writer.WriteAsync(message);
    }

    public ChannelReader<string> GetChannelReader()
    {
        return _channel.Reader;
    }
}
