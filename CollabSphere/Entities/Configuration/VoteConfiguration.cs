using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        // Cấu hình thuộc tính
        builder.Property(v => v.VoteType).IsRequired().HasMaxLength(10);

        // Cấu hình mối quan hệ 1-n: User - Votes
        builder.HasOne(v => v.User)
            .WithMany(u => u.Votes)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình mối quan hệ 1-n: Post - Votes (optional)
        builder.HasOne(v => v.Post)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PostId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình mối quan hệ 1-n: Comment - Votes (optional)
        builder.HasOne(v => v.Comment)
            .WithMany(c => c.Votes)
            .HasForeignKey(v => v.CommentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Đảm bảo mỗi user chỉ vote một lần cho mỗi post
        builder.HasIndex(v => new { v.UserId, v.PostId })
            .IsUnique()
            .HasFilter("[PostId] IS NOT NULL");

        // Đảm bảo mỗi user chỉ vote một lần cho mỗi comment
        builder.HasIndex(v => new { v.UserId, v.CommentId })
            .IsUnique()
            .HasFilter("[CommentId] IS NOT NULL");
    }
}
