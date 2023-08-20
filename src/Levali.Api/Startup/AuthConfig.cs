using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Levali.Api;

public static class AuthConfig
{
    public static void ConfigureAuth(this IServiceCollection services, ConfigurationManager configuration)
    {               
        services.AddAuthentication(a =>
        {
            a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(j =>
        {
            j.RequireHttpsMetadata = false;
            j.SaveToken = true;
            j.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["SecretKey"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();
    }
}
