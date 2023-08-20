namespace Levali.Core;

public interface IShortUrlRepository
{
    public Task Insert(ShortUrlEntity shortUrl);
    public Task<ShortUrlEntity?> GetByCode(string code);
    public Task UpdateAnalytics(string code);
    public Task Remove(string code);
}
