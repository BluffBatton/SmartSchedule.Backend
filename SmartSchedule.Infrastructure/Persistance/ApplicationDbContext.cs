using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Common;
using SmartSchedule.Domain.Entities;

namespace SmartSchedule.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<User> Users => Set<User>();

        public DbSet<Department> Departments => Set<Department>();

        public DbSet<TeacherSetting> TeacherSettings => Set<TeacherSetting>();  
        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<Notification> Notifications => Set<Notification>();

        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableFields();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditableFields()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(entry =>
                    entry.State == EntityState.Added ||
                    entry.State == EntityState.Modified ||
                    entry.State == EntityState.Deleted);

            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.UpdatedAtUtc = utcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = utcNow;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAtUtc = utcNow;
                    entry.Entity.UpdatedAtUtc = utcNow;
                }
            }
        }
    }
}
