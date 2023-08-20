using System.ComponentModel.DataAnnotations;

namespace Levali.Api;

public sealed class ShortenUrlRequest
{
    [Required(ErrorMessage = "The Url field is required.")]
    public string Url { get; set; } = string.Empty;
    public DateTime? Expiration { get; set; }

    public void Validate(Notification notification)
    {
        if (!Core.Url.TryParse(Url, out _))
        {
            notification.AddError($"{Url} is not a valid URL");
        }

        if (Expiration is not null && Expiration.Value < DateTime.UtcNow)
        {
            notification.AddError($"{nameof(Expiration)} should be cannot be earlier than now");
        }
    }
}
