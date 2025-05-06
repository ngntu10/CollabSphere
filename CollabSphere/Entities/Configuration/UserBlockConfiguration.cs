using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.Property(b => b.BlockedAt).IsRequired();

        // Cấu hình mối quan hệ với User (Blocker)
        builder.HasOne(b => b.Blocker)
            .WithMany(u => u.BlockedUsers)
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình mối quan hệ với User (Blocked)
        builder.HasOne(b => b.Blocked)
            .WithMany(u => u.BlockedByUsers)
            .HasForeignKey(b => b.BlockedId)
            .OnDelete(DeleteBehavior.Restrict);

        // Đảm bảo mỗi người chỉ block một người khác một lần
        builder.HasIndex(b => new { b.BlockerId, b.BlockedId }).IsUnique();
    }
}
