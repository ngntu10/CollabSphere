using CollabSphere.Modules.Email.Config;
using CollabSphere.Modules.User.Services;
using CollabSphere.Modules.User.Services.Impl;

namespace CollabSphere.Modules;

public static class AppModule
{
    public static IServiceCollection AddAppDependency(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddAuthServices(env);

        services.RegisterAutoMapper();

        return services;
    }

    private static void AddAuthServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<IUserService, UserService>();
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
