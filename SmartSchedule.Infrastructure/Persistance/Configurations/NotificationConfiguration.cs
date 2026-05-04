using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class NotificationConfiguration : BaseEntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.ToTable("Notifications");

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                .IsRequired();

            builder.Property(n => n.ReadAtUtc);

            builder.Property(n => n.ScheduledAtUtc);

            builder.Property(n => n.SentAtUtc);

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Booking)
                .WithMany(b => b.Notifications)
                .HasForeignKey(n => n.BookingId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}