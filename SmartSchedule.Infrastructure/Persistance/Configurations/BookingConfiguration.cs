using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class BookingConfiguration : BaseEntityConfiguration<Booking>
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            base.Configure(builder);

            builder.ToTable("Bookings");

            builder.Property(b => b.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.BookedAtUtc)
                .IsRequired();

            builder.Property(b => b.CancelledAtUtc);

            builder.Property(b => b.CancelReason)
                .HasMaxLength(500);

            builder.HasOne(b => b.Student)
                .WithMany(u => u.StudentBookings)
                .HasForeignKey(b => b.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.TimeSlot)
                .WithOne(ts => ts.Booking)
                .HasForeignKey<Booking>(b => b.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(b => b.TimeSlotId)
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");

            builder.HasIndex(b => new { b.StudentId, b.TimeSlotId })
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");
        }
    }
}