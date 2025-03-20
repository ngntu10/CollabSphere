using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Cấu hình thuộc tính
            builder.Property(n => n.Content).IsRequired().HasMaxLength(500);
            builder.Property(n => n.Link).HasMaxLength(255);
            builder.Property(n => n.NotificationType).IsRequired().HasMaxLength(50);

            // Cấu hình mối quan hệ 1-n: User - Notifications
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
