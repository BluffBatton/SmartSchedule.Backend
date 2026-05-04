using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class PasswordResetTokenConfiguration : BaseEntityConfiguration<PasswordResetToken>
    {
        public override void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            base.Configure(builder);

            builder.ToTable("PasswordResetTokens");

            builder.Property(t => t.TokenHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(t => t.TokenHash)
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");

            builder.Property(t => t.ExpiresAtUtc)
                .IsRequired();

            builder.Property(t => t.UsedAtUtc);

            builder.HasOne(t => t.User)
                .WithMany(u => u.PasswordResetTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}