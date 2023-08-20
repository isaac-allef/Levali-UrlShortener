using Levali.Core;
using Microsoft.AspNetCore.Mvc;

namespace Levali.Api;

[Route("v1/")]
[ApiController]
public class UserController : ControllerBase
{
    public readonly IUserRepository _userRepository;
    private readonly Notification _notification;
    
    public UserController(IUserRepository userRepository, Notification notification)
    {
        _userRepository = userRepository;
        _notification = notification;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] RegisterUserRequest request)
    {
        request.Validate(_notification);

        if (_notification.HasErrors())
        {
            return BadRequest(_notification.Errors);
        }
        
        var existingUser = await _userRepository.GetByEmail(request.Email);
        
        if (existingUser is not null)
        {
            return BadRequest("E-mail already used");
        }

        var newUser = new UserEntity(request.Name, request.Email, request.Password);
        await _userRepository.Insert(newUser);
        
        return Created(nameof(request.Name), request.Name);
    }
}
