namespace Levali.Core;

public sealed class ShortenUrlDto
{
    public Url Url { get; set; }
    public DateTime? Expiration { get; set; }
    public int UserId { get; set; }
}
