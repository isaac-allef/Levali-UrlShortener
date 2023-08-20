using System.ComponentModel.DataAnnotations;

namespace Levali.Api;

public sealed class AuthenticateUserRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Email { get; set; } = string.Empty;
    
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;

    public void Validate(Notification notification)
    {
        if (!Core.Email.TryParse(Email, out _))
        {
            notification.AddError($"{Email} is not a valid E-mail");
        }

        if (!Core.Password.TryParse(Password, out _))
        {
            notification.AddError($"{Password} is not a valid password");
        }
    }
}
