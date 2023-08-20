using Serilog;

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

        if (shortUrl.ExpirationAt is not null
            && shortUrl.ExpirationAt < DateTime.UtcNow)
        {
            await _enqueueer.EnqueueAsync(queueName: "expired", shortUrl);
            return null;
        }

        await _enqueueer.EnqueueAsync(queueName: "click", shortUrl);

        _logger
            .ForContext(nameof(code), code)
            .Information("Enlarge URL successfully");

        return shortUrl.TargetUrl;
    }
}
