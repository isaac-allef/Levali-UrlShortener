using Microsoft.Extensions.Hosting;
using Serilog.Context;

namespace Levali.Core;

public sealed class UpdateAnalyticsService : IHostedService, IDisposable
{
    private readonly IDequeueer _dequeueer;
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly Serilog.ILogger _logger;
    private Timer? _timer;

    public UpdateAnalyticsService(IDequeueer dequeueer, IShortUrlRepository shortUrlRepository, Serilog.ILogger logger)
    {
        _dequeueer = dequeueer;
        _shortUrlRepository = shortUrlRepository;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(
            async call => await ProcessQueue(),
            null,
            TimeSpan.FromMinutes(2),
            TimeSpan.FromSeconds(1)
        );
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private async Task ProcessQueue()
    {
        var shortUrl = await _dequeueer.DequeueAsync<ShortUrlEntity>("click");
        if (shortUrl is not null)
        {
            using (LogContext.PushProperty(nameof(shortUrl.UserId), shortUrl.UserId))
            {
                await _shortUrlRepository.UpdateAnalytics(shortUrl.Code);
                _logger
                    .ForContext(nameof(shortUrl.Code), shortUrl.Code)
                    .Information("Analytics updated successfully");
            }
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
