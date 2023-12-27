using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace MeganUploadFiles.Authentication
{
    public static class ConfigureAuthentificationServiceExtensions
    {
        public static void ConfigureJWT(this IServiceCollection services, bool IsDevelopment, string publicKeyJWT)
        {
            var AuthenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            AuthenticationBuilder.AddJwtBearer(options =>
            {
                options.Authority = "https://your-keycloak-server/auth/realms/your-realm";
                options.Audience = "your-client-id";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-client-secret"))
                };
            });
        }
    }
}
