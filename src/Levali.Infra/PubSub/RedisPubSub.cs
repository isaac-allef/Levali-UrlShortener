using Levali.Core;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace Levali.Infra;

public class RedisPubSub : IPublisher, Core.ISubscriber
{
    private readonly string _connectionString;
    private ConnectionMultiplexer? _connection;
    private StackExchange.Redis.ISubscriber? _subscriber;
    private readonly ILogger _logger;

    public RedisPubSub(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string channel, T value) where T : notnull
    {
        try
        {
            if (!IsConnected())
            {
                Connect();
            }

            if (_subscriber is null) throw new Exception("Pusblisher no connected");
            
            var message = JsonConvert.SerializeObject(value);
            await _subscriber.PublishAsync(channel, message);
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error redis publish");

            throw;
        }
    }

    public async Task SubscribeAsync<T>(string channel, Func<T, Task> run) where T : notnull
    {
        try
        {
            if (!IsConnected())
            {
                Connect();
            }

            if (_subscriber is null) throw new Exception("Subscriber no connected");

            await _subscriber.SubscribeAsync(channel, async (_, message) =>
            {
                var value = JsonConvert.DeserializeObject<T>(message.ToString());
                await run(value!);
            }, CommandFlags.None);
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error redis subscribe");
            
            throw;
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    private void Connect()
    {
        _connection = ConnectionMultiplexer.Connect(_connectionString);
        _subscriber = _connection.GetSubscriber();
    }

    private bool IsConnected()
        => _connection is not null && _subscriber is not null;
}
