namespace Levali.Api;

public sealed class ShortUrlViewer
{
    public string Code { get; private set; } = string.Empty;
    public string TargetUrl { get; private set; } = string.Empty;
    public DateTime? ExpirationAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int ClickCount { get; private set; }
    public DateTime? LastClick { get; private set; }
}
