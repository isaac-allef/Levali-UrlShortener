using Serilog;
using Serilog.Context;

namespace Levali.Core;

public sealed class EnlargeShortUrlService
{
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly IEnqueueer _enqueueer;
    private readonly ILogger _logger;

    public EnlargeShortUrlService(IShortUrlRepository shortUrlRepository, IEnqueueer enqueueer, ILogger logger)
    {
        _shortUrlRepository = shortUrlRepository;
        _enqueueer = enqueueer;
        _logger = logger;
    }

    public async Task<Url?> Execute(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentNullException(nameof(code));
        }
        
        var shortUrl = await _shortUrlRepository.GetByCode(code);

        if (shortUrl is null)
        {
            return null;
        }

        using (LogContext.PushProperty(nameof(shortUrl.UserId), shortUrl.UserId))
        using (LogContext.PushProperty(nameof(shortUrl.Code), shortUrl.Code))
        {
            if (shortUrl.ExpirationAt is not null
                && shortUrl.ExpirationAt < DateTime.UtcNow)
            {
                await LazyRemoveShortUrl(shortUrl.Code);
                return null;
            }

            await _enqueueer.EnqueueAsync(queueName: "click", shortUrl);

            _logger.Information("Enlarge URL successfully");

            return shortUrl.TargetUrl;
        }
    }

    private async Task LazyRemoveShortUrl(string code)
    {
        try
        {
            await _shortUrlRepository.Remove(code);
            _logger
                .ForContext(nameof(code), code)
                .Information("Expired short URL deleted successfully");
        }
        catch(Exception ex)
        {
            _logger
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error to delete expired short URL");
            
            throw;
        }
    }
}
