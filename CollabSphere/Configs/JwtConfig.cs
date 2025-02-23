using System.Security.Claims;
using System.Text;

using CollabSphere.Common;
using CollabSphere.Helpers;
using CollabSphere.Infrastructures.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

namespace CollabSphere.Configs;

public static class JwtConfig
{
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");

        var key = Encoding.ASCII.GetBytes(secretKey ?? "This is a secret key");

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.RequireHttpsMetadata = false;
            option.SaveToken = true;
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            option.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    var result =
                        JsonConvert.SerializeObject(ApiResult<string>.Failure(StatusCodes.Status401Unauthorized,
                            ["Unauthorized"]));

                    context.HandleResponse();
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;

                    await context.Response.WriteAsync(result);
                }
            };
        });
    }
}
