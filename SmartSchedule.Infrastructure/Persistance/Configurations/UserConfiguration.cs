using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("Users");

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.TeacherSetting)
                .WithOne(ts => ts.Teacher)
                .HasForeignKey<TeacherSetting>(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.TeacherTimeSlots)
                .WithOne(ts => ts.Teacher)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.StudentBookings)
                .WithOne(b => b.Student)
                .HasForeignKey(b => b.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.PasswordResetTokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.CreatedByAdmin)
                .WithMany(u => u.CreatedUsers)
                .HasForeignKey(u => u.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}