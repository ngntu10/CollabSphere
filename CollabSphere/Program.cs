using CollabSphere;
using CollabSphere.Configs;
using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Filters;
using CollabSphere.Middleware;
using CollabSphere.Modules;
using CollabSphere.Modules.Auth.Services;
using CollabSphere.Modules.Auth.Services.Impl;
using CollabSphere.Modules.Chat.Hubs;
using CollabSphere.Modules.Chat.Services.Impl;
using CollabSphere.Modules.Chat.Services.Interfaces;
using CollabSphere.Modules.Notification.Mapping;
using CollabSphere.Modules.Notification.Service;
using CollabSphere.Modules.Posts.Service;
using CollabSphere.Modules.Posts.Service.Imp;
using CollabSphere.Modules.User.Service;
using CollabSphere.Shared;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(
    config => config.Filters.Add(typeof(ValidateModelAttribute))
);

builder.Services.AddConfig(builder.Configuration);

builder.Services.AddSharedService(builder.Environment);

builder.Services.AddAppDependency(builder.Environment);

builder.Services.AddEmailConfiguration(builder.Configuration);

builder.Services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddProblemDetails();
builder.Services.AddScoped<IPostService, PostService>();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAutoMapper(typeof(NotificationMappingProfile).Assembly);

var app = builder.Build();

// using var scope = app.Services.CreateScope();
// await AutomatedMigration.MigrateAsync(scope.ServiceProvider);

app.MapHub<ChatHub>("/chathub");

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CollabSphere V1"); });

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<PerformanceMiddleware>();

app.UseMiddleware<TransactionMiddleware>();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

namespace CollabSphere
{
    public partial class Program { }
}
