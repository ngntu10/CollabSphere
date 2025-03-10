using CollabSphere.Modules.Auth.Services;
using CollabSphere.Modules.Auth.Services.Impl;
using CollabSphere.Modules.Email.Config;
using CollabSphere.Modules.TodoItem.Services;
using CollabSphere.Modules.TodoItem.Services.Impl;
using CollabSphere.Modules.TodoList.Services;
using CollabSphere.Modules.TodoList.Services.Impl;

namespace CollabSphere.Modules;

public static class AppModule
{
    public static IServiceCollection AddAppDependency(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddAuthServices(env);

        services.AddSampleServices(env);

        services.RegisterAutoMapper();

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
    }

    private static void RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IModuleMarker));
    }

    public static void AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("SmtpSettings").Get<SmtpSettings>());
    }
}
