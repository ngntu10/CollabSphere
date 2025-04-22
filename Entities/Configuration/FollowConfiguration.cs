using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        // Sửa từ snake_case sang PascalCase
        builder.Property(f => f.FollowedAt).IsRequired();

        // Cấu hình mối quan hệ many-to-many giữa User với chính nó thông qua Follow
        builder.HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Đảm bảo mỗi người chỉ follow một người khác một lần
        builder.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique();
    }
}
