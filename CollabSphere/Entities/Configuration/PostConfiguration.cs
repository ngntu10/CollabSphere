using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        // Cấu hình thuộc tính
        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Content).IsRequired();
        builder.Property(p => p.Category).IsRequired().HasDefaultValue("General");

        // Cấu hình mối quan hệ 1-n: User - Posts
        builder.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
