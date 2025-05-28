using System.Text;

using CollabSphere.Modules.Auth.Services;
using CollabSphere.Modules.Auth.Services.Impl;
using CollabSphere.Modules.Comment.Services;
using CollabSphere.Modules.Email.Config;
using CollabSphere.Modules.Follow;
using CollabSphere.Modules.TodoItem.Services;
using CollabSphere.Modules.TodoItem.Services.Impl;
using CollabSphere.Modules.TodoList.Services;
using CollabSphere.Modules.TodoList.Services.Impl;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CollabSphere.Modules;

public static class AppModule
{
    public static IServiceCollection AddAppDependency(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddAuthServices(env);

        services.AddSampleServices(env);

        services.AddFollowModule();

        services.RegisterAutoMapper();

        services.AddJwtAuthentication();

        return services;
    }

    private static void AddAuthServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<IAuthService, AuthService>();
    }

    private static void AddSampleServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<ITodoItemService, TodoItemService>();
        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ICommentService, CommentServices>();
    }

    private static void RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IModuleMarker));
    }

    public static void AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("SmtpSettings").Get<SmtpSettings>());
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        return services;
    }
}
