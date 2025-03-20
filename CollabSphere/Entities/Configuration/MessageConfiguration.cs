using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // Cấu hình thuộc tính
        builder.Property(m => m.Subject).HasMaxLength(100);
        builder.Property(m => m.Content).IsRequired().HasMaxLength(5000);
        builder.Property(m => m.MessageType).IsRequired().HasMaxLength(20);

        // Cấu hình mối quan hệ 1-n: User (Sender) - SentMessages
        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình mối quan hệ 1-n: User (Receiver) - ReceivedMessages
        builder.HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
