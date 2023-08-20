using Serilog;

namespace Levali.Core;

public sealed class ShortenUrlService
{
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly ILogger _logger;

    public ShortenUrlService(IShortUrlRepository shortUrlRepository, ILogger logger)
    {
        _shortUrlRepository = shortUrlRepository;
        _logger = logger;
    }

    public async Task<string> Execute(ShortenUrlDto dto)
    {
        var code = CodeGenerator.Generate();

        var shortUrl = new ShortUrlEntity(code, dto.UserId, dto.Url);

        if (dto.Expiration is not null)
        {
            shortUrl.ExpiresIn(dto.Expiration.Value);
        }

        await _shortUrlRepository.Insert(shortUrl);

        _logger
            .ForContext(nameof(dto.UserId), dto.UserId)
            .ForContext(nameof(code), code)
            .Information("Shorten URL successfully");

        return shortUrl.Code;
    }
}
