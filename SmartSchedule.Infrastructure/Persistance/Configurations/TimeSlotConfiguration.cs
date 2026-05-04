using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class TimeSlotConfiguration : BaseEntityConfiguration<TimeSlot>
    {
        public override void Configure(EntityTypeBuilder<TimeSlot> builder)
        {
            base.Configure(builder);

            builder.ToTable("TimeSlots");

            builder.Property(ts => ts.StartAtUtc)
                .IsRequired();

            builder.Property(ts => ts.EndAtUtc)
                .IsRequired();

            builder.Property(ts => ts.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(ts => ts.Teacher)
                .WithMany(u => u.TeacherTimeSlots)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ts => new { ts.TeacherId, ts.StartAtUtc, ts.EndAtUtc })
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");
        }
    }
}