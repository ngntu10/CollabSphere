using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class SubredditConfiguration : IEntityTypeConfiguration<Subreddit>
{
    public void Configure(EntityTypeBuilder<Subreddit> builder)
    {
        // Cấu hình thuộc tính
        builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.ThumbnailUrl).HasMaxLength(255);
        builder.Property(s => s.Rules).HasMaxLength(2000);

        // Đảm bảo tên subreddit là duy nhất
        builder.HasIndex(s => s.Name).IsUnique();

        // Cấu hình mối quan hệ 1-n: User - Created Subreddits
        builder.HasOne(s => s.Creator)
            .WithMany() // Không có navigation property từ User
            .HasForeignKey(s => s.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
