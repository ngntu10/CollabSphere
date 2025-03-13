using CollabSphere.Entities.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabSphere.Entities.Configuration;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.ExpirationDate)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithOne(u => u.EmailVerificationToken)
            .HasForeignKey<EmailVerificationToken>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Token).IsUnique();

        builder.ToTable("EmailVerificationTokens");
    }
}
