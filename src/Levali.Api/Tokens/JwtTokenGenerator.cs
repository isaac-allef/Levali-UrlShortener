using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Levali.Api;

public sealed class JwtTokenGenerator
{
    private readonly string _secret;

    public JwtTokenGenerator(string secret)
    {
        _secret = secret;
    }

    public Token Generate(string userId, int expiresInMinutes)
    {
        var secretKey = Encoding.ASCII.GetBytes(_secret);
        var expiresIn = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Name, userId)
            }),
            Expires = expiresIn,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new Token()
        {
            Value = tokenHandler.WriteToken(token),
            ExpiresIn = expiresIn
        };
    }
}
