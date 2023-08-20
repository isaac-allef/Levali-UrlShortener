using Microsoft.AspNetCore.Mvc;
using Levali.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Levali.Api;

[Route("")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly Lazy<ShortenUrlService> _shortenUrl;
    private readonly Lazy<EnlargeShortUrlService> _enlargeShortUrl;
    private readonly Notification _notification;

    public UrlController(Lazy<ShortenUrlService> shortenUrl, Lazy<EnlargeShortUrlService> enlargeShortUrl, Notification notification)
    {
        _shortenUrl = shortenUrl;
        _enlargeShortUrl = enlargeShortUrl;
        _notification = notification;
    }

    [HttpPost]
    [Route("v1/shorten")]
    [Authorize]
    public async Task<IActionResult> ShortenAsync(
        [FromBody] ShortenUrlRequest request)
    {
        request.Validate(_notification);

        if (_notification.HasErrors())
        {
            return BadRequest(_notification.Errors);
        }

        var dto = new ShortenUrlDto()
        {
            Url = request.Url,
            Expiration = request.Expiration,
            UserId = int.Parse(User.Identity?.Name!)
        };

        var shortUrl = await _shortenUrl.Value.Execute(dto);
        
        return Ok(shortUrl);
    }

    [HttpGet]
    [Route("/{code}")]
    public async Task<IActionResult> EnlargeAsync(
        [FromRoute][Required(AllowEmptyStrings = false)] string code)
    {
        var url = await _enlargeShortUrl.Value.Execute(code);

        if (url is null)
        {
            return NotFound();
        }
        
        return Redirect(url);
    }
}
