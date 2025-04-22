using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            // Sửa từ snake_case sang PascalCase
            builder.Property(s => s.SubscribedAt).IsRequired();

            // Cấu hình mối quan hệ 1-n: User - Subscriptions
            builder.HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình mối quan hệ 1-n: Subreddit - Subscriptions
            builder.HasOne(s => s.Subreddit)
                .WithMany(s => s.Subscriptions)
                .HasForeignKey(s => s.SubredditId)
                .OnDelete(DeleteBehavior.Cascade);

            // Đảm bảo mỗi user chỉ subscribe một lần vào mỗi subreddit
            builder.HasIndex(s => new { s.UserId, s.SubredditId }).IsUnique();
        }
    }
}
