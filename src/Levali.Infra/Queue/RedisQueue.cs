using Levali.Core;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace Levali.Infra;

public class RedisQueue : IEnqueueer, IDequeueer
{
    private readonly string _connectionString;
    private ConnectionMultiplexer? _connection;
    private IDatabase? _database;
    private readonly ILogger _logger;

    public RedisQueue(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task EnqueueAsync<T>(string queueName, T item) where T : notnull
    {
        try
        {
            if (!IsConnected())
            {
                Connect();
            }

            var serializedItem = JsonConvert.SerializeObject(item);
            await _database!.ListLeftPushAsync(queueName, serializedItem);
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error redis queue publish");

            throw;
        }
    }

    public async Task<T?> DequeueAsync<T>(string queueName)
    {
        try
        {
            if (!IsConnected())
            {
                Connect();
            }
            
            string? serializedItem = await _database!.ListRightPopAsync(queueName);
            if (serializedItem is not null)
            {
                var item = JsonConvert.DeserializeObject<T>(serializedItem);
                return item;
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error redis queue subscribe");
            
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
        _database = _connection.GetDatabase();
    }

    private bool IsConnected()
        => _connection is not null && _database is not null;
}
