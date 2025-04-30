using System.Reflection;
using CollabSphere.Modules.User.Models;
using CollabSphere.Modules.User.Service;
using CollabSphere.Modules.User.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CollabSphere.Modules.User
{
    public static class UserModule
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services)
        {
            // Đăng ký services
            services.AddScoped<IUserService, UserService>();

            // Đăng ký validators
            services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
            services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();

            // Đăng ký AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}