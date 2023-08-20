using Microsoft.Extensions.Hosting;
using Serilog.Context;

namespace Levali.Core;

public sealed class LazyRemoveService : IHostedService, IDisposable
{
    private readonly ISubscriber _subscriber;
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly Serilog.ILogger _logger;

    public LazyRemoveService(ISubscriber subscriber, IShortUrlRepository shortUrlRepository, Serilog.ILogger logger)
    {
        _subscriber = subscriber;
        _shortUrlRepository = shortUrlRepository;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync<ShortUrlEntity>("expired", async (shortUrl) =>
        {
            using (LogContext.PushProperty(nameof(shortUrl.UserId), shortUrl.UserId))
            {
                await _shortUrlRepository.Remove(shortUrl.Code);
                _logger
                    .ForContext(nameof(shortUrl.Code), shortUrl.Code)
                    .Information("Expired short URL deleted successfully");
            }
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}
