using System;

using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class PostImageConfiguration : IEntityTypeConfiguration<PostImages>
{
    public void Configure(EntityTypeBuilder<PostImages> builder)
    {
        builder.HasOne(p => p.Post) // Mối quan hệ 1-n với Post
           .WithMany(p => p.PostImages) // Post có nhiều PostImages
           .HasForeignKey(p => p.PostId) // Đặt 'PostId' là khóa ngoại
           .HasPrincipalKey(p => p.Id);

    }
}
