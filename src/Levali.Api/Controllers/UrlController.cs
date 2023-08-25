using Microsoft.AspNetCore.Mvc;
using Levali.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Levali.Infra;
using Dapper;

namespace Levali.Api;

[Route("")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly Lazy<ShortenUrlService> _shortenUrl;
    private readonly Lazy<EnlargeShortUrlService> _enlargeShortUrl;
    private readonly Notification _notification;
    private readonly DbSession _session;

    public UrlController(
        Lazy<ShortenUrlService> shortenUrl, 
        Lazy<EnlargeShortUrlService> enlargeShortUrl, 
        Notification notification, 
        DbSession sesstion)
    {
        _shortenUrl = shortenUrl;
        _enlargeShortUrl = enlargeShortUrl;
        _notification = notification;
        _session = sesstion;
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
    [Route("v1/short-url/{code}")]
    [Authorize]
    public async Task<IActionResult> GetAsync(
        [FromRoute][Required(AllowEmptyStrings = false)] string code)
    {
        var userId = int.Parse(User.Identity?.Name!);
        
        var query = $"SELECT Code, TargetUrl, ExpirationAt, CreatedAt, ClickCount, LastClick FROM master.dbo.ShortUrls INNER JOIN master.dbo.ShortUrlsAnalytics ON (ShortUrlCode = Code) WHERE UserId = userId AND Code = @code";
        var shortUrl = await _session.Connection
                        .QuerySingleOrDefaultAsync<ShortUrlViewer>(query, new { userId, code });
        
        if (shortUrl is null)
        {
            return NotFound();
        }

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
