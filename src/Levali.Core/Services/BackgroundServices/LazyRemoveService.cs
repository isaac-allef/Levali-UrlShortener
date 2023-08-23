using Microsoft.Extensions.Hosting;
using Serilog.Context;

namespace Levali.Core;

public sealed class LazyRemoveService : IHostedService, IDisposable
{
    private readonly IDequeueer _dequeueer;
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly Serilog.ILogger _logger;
    private Timer? _timer;

    public LazyRemoveService(IDequeueer dequeueer, IShortUrlRepository shortUrlRepository, Serilog.ILogger logger)
    {
        _dequeueer = dequeueer;
        _shortUrlRepository = shortUrlRepository;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(
            async callback => await ProcessQueue(),
            null,
            dueTime: TimeSpan.FromSeconds(30),
            period: TimeSpan.FromSeconds(5)
        );
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private async Task ProcessQueue()
    {
        var shortUrl = await _dequeueer.DequeueAsync<ShortUrlEntity>("expired");
        if (shortUrl is not null)
        {
            using (LogContext.PushProperty(nameof(shortUrl.UserId), shortUrl.UserId))
            {
                await _shortUrlRepository.Remove(shortUrl.Code);
                _logger
                    .ForContext(nameof(shortUrl.Code), shortUrl.Code)
                    .Information("Expired short URL deleted successfully");
            }
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
