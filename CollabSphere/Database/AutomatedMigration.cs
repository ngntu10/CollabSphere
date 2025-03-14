using CollabSphere.Entities.Domain;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Database;

public static class AutomatedMigration
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<DatabaseContext>();

        if (context.Database.IsMySql()) await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<User>>();

        await DatabaseContextSeed.SeedDatabaseAsync(context, userManager);
    }
}
