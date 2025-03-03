using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration
{
    public class VideoCallConfiguration : IEntityTypeConfiguration<VideoCall>
    {
        public void Configure(EntityTypeBuilder<VideoCall> builder)
        {
            // Cấu hình thuộc tính
            builder.Property(v => v.CallStatus).IsRequired().HasMaxLength(20);
            builder.Property(v => v.CallSessionUrl).HasMaxLength(255);

            // Cấu hình mối quan hệ 1-n: User (Caller) - VideoCalls
            builder.HasOne(v => v.Caller)
                .WithMany(u => u.VideoCalls)
                .HasForeignKey(v => v.CallerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ 1-n: User (Receiver) - (không có navigation property)
            builder.HasOne(v => v.Receiver)
                .WithMany()
                .HasForeignKey(v => v.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
