using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        // Cấu hình thuộc tính
        builder.Property(r => r.Reason).IsRequired().HasMaxLength(500);
        builder.Property(r => r.Status).IsRequired().HasMaxLength(20);

        // Cấu hình mối quan hệ 1-n: User - Reports
        builder.HasOne(r => r.Reporter)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình mối quan hệ 1-n: Post - Reports (optional)
        builder.HasOne(r => r.ReportedPost)
            .WithMany(p => p.Reports)
            .HasForeignKey(r => r.ReportedPostId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình mối quan hệ 1-n: Comment - Reports (optional)
        builder.HasOne(r => r.ReportedComment)
            .WithMany(c => c.Reports)
            .HasForeignKey(r => r.ReportedCommentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
