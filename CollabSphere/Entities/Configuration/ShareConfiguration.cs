using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class ShareConfiguration : IEntityTypeConfiguration<Share>
{
    public void Configure(EntityTypeBuilder<Share> builder)
    {
        // Cấu hình thuộc tính
        builder.Property(s => s.ShareComment).HasMaxLength(500);

        // Cấu hình mối quan hệ 1-n: User - Shares
        builder.HasOne(s => s.User)
            .WithMany(u => u.Shares)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình mối quan hệ 1-n: Post - Shares
        builder.HasOne(s => s.Post)
            .WithMany(p => p.Shares)
            .HasForeignKey(s => s.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
