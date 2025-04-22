
using CollabSphere.Entities.Domain;

using Microsoft.AspNetCore.Identity;

namespace CollabSphere.Database;

public static class DatabaseContextSeed
{
    public static async Task SeedDatabaseAsync(DatabaseContext context, UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new User { UserName = "admin", Email = "admin@admin.com", EmailConfirmed = true };

            await userManager.CreateAsync(user, "Admin123.?");
        }

        await context.SaveChangesAsync();
    }
}
