using Levali.Core;
using Microsoft.AspNetCore.Mvc;

namespace Levali.Api;

[Route("v1/")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    public readonly IUserRepository _userRepository;
    public readonly JwtTokenGenerator _generateToken;
    private readonly Notification _notification;
    
    public AuthenticationController(IUserRepository userRepository, JwtTokenGenerator generateToken, Notification notification)
    {
        _userRepository = userRepository;
        _generateToken = generateToken;
        _notification = notification;
    }

    [HttpPost]
    [Route("authenticate")]
    public async Task<IActionResult> AuthenticateAsync(
        [FromBody] AuthenticateUserRequest request)
    {
        request.Validate(_notification);

        if (_notification.HasErrors())
        {
            return BadRequest(_notification.Errors);
        }

        var user = await _userRepository.GetByEmail(request.Email);

        if (user is null || !UserEntity.GenerateHash(request.Password).Equals(user.PasswordHash))
        {
            return Unauthorized(request.Email);
        }

        var token = _generateToken.Generate(user.Id.ToString(), expiresInMinutes: 20);

        if (token is null)
        {
            return Unauthorized(request.Email);
        }

        return Ok(token);
    }
}
