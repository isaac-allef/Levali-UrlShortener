namespace Levali.Core;

public class ShortUrlAnalyticsEntity
{
    public int Id { get; private set; }
    public string ShortUrlCode { get; private set; }
    public int ClickCount { get; private set; }
    public DateTime? LastClick { get; private set; }

    public ShortUrlAnalyticsEntity(string shortUrlCode) : this(0, shortUrlCode, 0, null)
    {
    }
    
    private ShortUrlAnalyticsEntity(int id, string shortUrlCode, int clickCount, DateTime? lastClick)
    {
        Id = id;
        ShortUrlCode = shortUrlCode;
        ClickCount = clickCount;
        LastClick = lastClick;
    }

    public void MarkClick()
    {
        ClickCount += 1;
        LastClick = DateTime.UtcNow;
    }
}
