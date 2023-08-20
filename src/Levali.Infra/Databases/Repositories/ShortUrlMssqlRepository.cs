using Dapper;
using Levali.Core;
using Serilog;

namespace Levali.Infra;

public class ShortUrlMssqlRepository : IShortUrlRepository
{
    private readonly DbSession _session;
    private readonly ILogger _logger;

    public ShortUrlMssqlRepository(DbSession session, ILogger logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task Insert(ShortUrlEntity shortUrl)
    {
        try
        {
            using var transaction = _session.Connection.BeginTransaction();
            var query = $"INSERT INTO master.dbo.ShortUrls (Code, TargetUrl, ExpirationAt, CreatedAt, UserId) VALUES (@Code, @TargetUrl, @ExpirationAt, @CreatedAt, @UserId);";
            await _session.Connection.ExecuteAsync(query, new
            {
                shortUrl.Code,
                TargetUrl = shortUrl.TargetUrl.ToString(),
                shortUrl.ExpirationAt,
                shortUrl.CreatedAt,
                shortUrl.UserId
            }, transaction);

            var shortUrlAnalytics = new ShortUrlAnalyticsEntity(shortUrlCode: shortUrl.Code);
            var query2 = $"INSERT INTO master.dbo.ShortUrlsAnalytics (ShortUrlCode, ClickCount, LastClick) VALUES (@ShortUrlCode, @ClickCount, @LastClick);";
            await _session.Connection.ExecuteAsync(query2, shortUrlAnalytics, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(shortUrl.UserId), shortUrl.UserId)
                .ForContext(nameof(shortUrl.Code), shortUrl.Code)
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error insert short URL");

            throw;
        }
    }

    public async Task<ShortUrlEntity?> GetByCode(string code)
    {
        try
        {
            var query = $"SELECT Code, UserId, TargetUrl, ExpirationAt, CreatedAt FROM master.dbo.ShortUrls WHERE Code = @code";
            return await _session.Connection.QuerySingleOrDefaultAsync<ShortUrlEntity>(query, new { code });
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(code), code)
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error get short URL by code");

            return default;
        }
    }

    public async Task UpdateAnalytics(string code)
    {
        try
        {
            using var transaction = _session.Connection.BeginTransaction();
            var selectQuery = "SELECT Id, ShortUrlCode, ClickCount, LastClick FROM master.dbo.ShortUrlsAnalytics WITH (UPDLOCK) WHERE ShortUrlCode = @ShortUrlCode;";
            var shortUrlAnalytics = await _session.Connection.QuerySingleAsync<ShortUrlAnalyticsEntity>(selectQuery, new { ShortUrlCode = code }, transaction);

            shortUrlAnalytics.MarkClick();

            var query = $"UPDATE master.dbo.ShortUrlsAnalytics SET ClickCount = @ClickCount, LastClick = @LastClick WHERE ShortUrlCode = @ShortUrlCode;";
            await _session.Connection.ExecuteAsync(query, shortUrlAnalytics, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(code), code)
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Update analytics error");
            
            throw;
        }
    }

    public async Task Remove(string code)
    {
        try
        {
            var query = $"DELETE FROM master.dbo.ShortUrls WHERE Code = @code;";
            await _session.Connection.ExecuteAsync(query, new { code });
        }
        catch(Exception ex)
        {
            _logger
                .ForContext(nameof(code), code)
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error delete short url");
            
            throw;
        }
    }
}
