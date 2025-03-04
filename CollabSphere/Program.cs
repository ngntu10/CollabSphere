using CollabSphere;
using CollabSphere.Configs;
using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Filters;
using CollabSphere.Middleware;
using CollabSphere.Modules;
using CollabSphere.Shared;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(
    config => config.Filters.Add(typeof(ValidateModelAttribute))
);

builder.Services.AddConfig(builder.Configuration);

builder.Services.AddSharedService(builder.Environment);

builder.Services.AddAppDependency(builder.Environment);

builder.Services.AddEmailConfiguration(builder.Configuration);

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddProblemDetails();

var app = builder.Build();

using var scope = app.Services.CreateScope();

await AutomatedMigration.MigrateAsync(scope.ServiceProvider);

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CollabSphere V1"); });

app.UseHttpsRedirection();

app.UseCors(corsPolicyBuilder =>
    corsPolicyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
);

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
