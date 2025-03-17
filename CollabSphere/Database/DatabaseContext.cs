using System.Reflection;

using CollabSphere.Common;
using CollabSphere.Entities.Domain;
using CollabSphere.Helpers;
using CollabSphere.Shared.Claim;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Database;

public class DatabaseContext : IdentityDbContext<User, Role, Guid>
{
    private readonly IClaimService _claimService;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IClaimService claimService) : base(options)
    {
        _claimService = claimService;
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    public DbSet<TodoList> TodoLists { get; set; }

    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Follow> Follows { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<Report> Reports { get; set; }

    public DbSet<Share> Shares { get; set; }

    public DbSet<Subreddit> Subreddits { get; set; }

    public DbSet<Subscription> Subscriptions { get; set; }

    public DbSet<VideoCall> VideoCalls { get; set; }

    public DbSet<Vote> Votes { get; set; }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditedEntity>())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = ParseGuidString.ParseGuid(_claimService.GetUserId());
                    entry.Entity.CreatedOn = DateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedBy = ParseGuidString.ParseGuid(_claimService.GetUserId());
                    entry.Entity.UpdatedOn = DateTime.Now;
                    break;
            }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
