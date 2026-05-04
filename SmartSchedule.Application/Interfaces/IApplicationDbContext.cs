using Microsoft.EntityFrameworkCore;
using SmartSchedule.Domain.Entities;

namespace SmartSchedule.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Department> Departments { get; }
        DbSet<TeacherSetting> TeacherSettings { get; }
        DbSet<TimeSlot> TimeSlots { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<PasswordResetToken> PasswordResetTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
