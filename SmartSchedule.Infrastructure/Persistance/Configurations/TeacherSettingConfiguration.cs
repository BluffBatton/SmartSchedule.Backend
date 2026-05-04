using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class TeacherSettingConfiguration : BaseEntityConfiguration<TeacherSetting>
    {
        public override void Configure(EntityTypeBuilder<TeacherSetting> builder)
        {
            base.Configure(builder);

            builder.ToTable("TeacherSettings");

            builder.Property(ts => ts.WorkDays)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ts => ts.DefaultStartTime)
                .IsRequired();

            builder.Property(ts => ts.DefaultEndTime)
                .IsRequired();

            builder.Property(ts => ts.ConsultationDurationMinutes)
                .IsRequired();

            builder.Property(ts => ts.IsActive)
                .IsRequired();

            builder.HasOne(ts => ts.Teacher)
                .WithOne(u => u.TeacherSetting)
                .HasForeignKey<TeacherSetting>(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ts => ts.TeacherId)
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");
        }
    }
}