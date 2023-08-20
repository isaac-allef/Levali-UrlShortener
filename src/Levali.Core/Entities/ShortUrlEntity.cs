namespace Levali.Core;

public sealed class ShortUrlEntity
{
    public string Code { get; private set; }
    public int UserId { get; private set; }
    public Url TargetUrl { get; private set; }
    public DateTime? ExpirationAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void ExpiresIn(DateTime expiration)
    {
        if (expiration < DateTime.UtcNow)
        {
            throw new ArgumentException("should be cannot be earlier than now", nameof(expiration));
        }

        ExpirationAt = expiration;
    }

    public ShortUrlEntity(string code, int userId, Url targetUrl) : this(code, userId, targetUrl, DateTime.UtcNow)
    {
    }

    //
    // This private constructor is here because it is used by Dapper to deserialize all your parameters
    private ShortUrlEntity(
        string code, int userId, string targetUrl, DateTime? expirationAt, DateTime createdAt) 
        : this(code, userId, targetUrl, createdAt)
    {
        ExpirationAt = expirationAt;
    }

    private ShortUrlEntity(string code, int userId, Url targetUrl, DateTime createdAt)
    {
        Code = code;
        UserId = userId;
        TargetUrl = targetUrl;
        CreatedAt = createdAt;
    }
}
