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
using CollabSphere.Modules.Chat.Services.Interfaces;
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

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // ⚠️ Chỉ định frontend của bạn
                .AllowCredentials() // ⚠️ Quan trọng để gửi cookie
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// builder.Services.AddScoped<IChatService, ChatService>();
// builder.Services.AddScoped<IMessageService, MessageService>();

// SignalR configuration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400; // 100 KB
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});


var app = builder.Build();

using var scope = app.Services.CreateScope();

await AutomatedMigration.MigrateAsync(scope.ServiceProvider);

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
