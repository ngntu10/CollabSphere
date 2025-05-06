using CollabSphere.Modules.Follow.Service;
using CollabSphere.Modules.Follow.Service.Imp;

namespace CollabSphere.Modules.Follow
{
    public static class FollowModule
    {
        public static IServiceCollection AddFollowModule(this IServiceCollection services)
        {
            services.AddScoped<IFollowService, FollowService>();

            return services;
        }
    }
}
