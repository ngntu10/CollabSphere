using CollabSphere.Modules.Email.Services;
using CollabSphere.Modules.Email.Services.DevImpl;
using CollabSphere.Modules.Email.Services.Impl;
using CollabSphere.Modules.Template.Services;
using CollabSphere.Modules.Template.Services.Impl;
using CollabSphere.Shared.Claim;

namespace CollabSphere.Shared;

public static class SharedServiceDependencyInjection
{
    public static IServiceCollection AddSharedService(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddServices(env);

        return services;
    }

    private static void AddServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<IClaimService, ClaimService>();

        services.AddScoped<ITemplateService, TemplateService>();

        if (env.IsDevelopment())
            services.AddScoped<IEmailService, DevEmailService>();
        else
            services.AddScoped<IEmailService, EmailService>();
    }
}
