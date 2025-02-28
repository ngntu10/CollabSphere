using CollabSphere;
using CollabSphere.Configs;
using CollabSphere.Database;
using CollabSphere.Exceptions;
using CollabSphere.Filters;
using CollabSphere.Middleware;
using CollabSphere.Modules;
using CollabSphere.Shared;

using FluentValidation;
using FluentValidation.AspNetCore;

using MySqlConnector;

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
CheckMySqlConnection();

app.Run();
void CheckMySqlConnection()
{
    string connectionString = "Server=localhost;Database=collabsphere;User=root;Password=StrongPass@123;";
    using (MySqlConnection conn = new MySqlConnection(connectionString))
    {
        try
        {
            conn.Open();
            Console.WriteLine("✅ Kết nối MySQL thành công!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Lỗi kết nối MySQL: " + ex.Message);
        }
    }
}

namespace CollabSphere
{
    public partial class Program { }
}

