using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CollabSphere.Entities.Domain;

using Microsoft.IdentityModel.Tokens;

namespace CollabSphere.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(User user, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");

        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static ClaimsPrincipal ValidateToken(string token, IConfiguration configuration)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public static (ClaimsPrincipal principal, DateTime expiresAt) ValidateTokenWithExpiration(string token, IConfiguration configuration)
    {
        if (string.IsNullOrEmpty(token))
            return (null, DateTime.MinValue);

        try
        {
            var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            var expiryClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expiryClaim != null && long.TryParse(expiryClaim.Value, out var expiryTimestamp))
            {
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryTimestamp).UtcDateTime;
                return (principal, expiryDate);
            }

            return (principal, DateTime.MinValue);
        }
        catch
        {
            return (null, DateTime.MinValue);
        }
    }
}
