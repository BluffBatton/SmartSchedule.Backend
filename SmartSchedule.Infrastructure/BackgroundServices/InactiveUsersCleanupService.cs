using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSchedule.Domain.Enums;
using SmartSchedule.Infrastructure.Persistance;

namespace SmartSchedule.Infrastructure.BackgroundServices
{
    public sealed class InactiveUsersCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptionsMonitor<InactiveUsersCleanupOptions> _optionsMonitor;
        private readonly ILogger<InactiveUsersCleanupService> _logger;

        public InactiveUsersCleanupService(
            IServiceScopeFactory scopeFactory,
            IOptionsMonitor<InactiveUsersCleanupOptions> optionsMonitor,
            ILogger<InactiveUsersCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _optionsMonitor = optionsMonitor;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var initialOptions = _optionsMonitor.CurrentValue;

            if (!initialOptions.Enabled)
            {
                _logger.LogInformation("InactiveUsersCleanupService is disabled by configuration.");
                return;
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(Math.Max(0, initialOptions.InitialDelaySeconds)),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var options = _optionsMonitor.CurrentValue;

                if (!options.Enabled)
                {
                    _logger.LogInformation("InactiveUsersCleanupService disabled at runtime, sleeping until next check.");
                }
                else
                {
                    try
                    {
                        await RunCleanupAsync(options, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Inactive users cleanup iteration failed.");
                    }
                }

                var interval = TimeSpan.FromHours(Math.Max(1, options.RunIntervalHours));

                try
                {
                    await Task.Delay(interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task RunCleanupAsync(InactiveUsersCleanupOptions options, CancellationToken cancellationToken)
        {
            var thresholdDays = Math.Max(1, options.InactivityThresholdDays);
            var batchSize = Math.Clamp(options.BatchSize, 1, 1000);
            var cutoffUtc = DateTime.UtcNow.AddDays(-thresholdDays);

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var candidateIds = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Role != UserRole.Admin)
                .Where(u =>
                    (u.LastLoginAtUtc == null && u.CreatedAtUtc < cutoffUtc) ||
                    (u.LastLoginAtUtc != null && u.LastLoginAtUtc < cutoffUtc))
                .Where(u =>
                    u.Role != UserRole.Teacher ||
                    (!u.TeacherTimeSlots.Any(ts =>
                            ts.StartAtUtc > DateTime.UtcNow &&
                            ts.Status != TimeSlotStatus.Cancelled) &&
                     !u.TeacherTimeSlots.Any(ts =>
                            ts.Booking != null &&
                            ts.Booking.Status == BookingStatus.Active &&
                            ts.StartAtUtc > DateTime.UtcNow)))
                .Where(u =>
                    !u.StudentBookings.Any(b =>
                        b.Status == BookingStatus.Active &&
                        b.TimeSlot.StartAtUtc > DateTime.UtcNow))
                .OrderBy(u => u.LastLoginAtUtc ?? u.CreatedAtUtc)
                .Take(batchSize)
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            if (candidateIds.Count == 0)
            {
                _logger.LogDebug("InactiveUsersCleanup: no candidates older than {Cutoff:O}.", cutoffUtc);
                return;
            }

            _logger.LogInformation(
                "InactiveUsersCleanup: hard-deleting {Count} inactive users (cutoff {Cutoff:O}).",
                candidateIds.Count,
                cutoffUtc);

            await HardDeleteUsersAsync(dbContext, candidateIds, cancellationToken);
        }

        private static async Task HardDeleteUsersAsync(
            ApplicationDbContext dbContext,
            IReadOnlyCollection<Guid> userIds,
            CancellationToken cancellationToken)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            await dbContext.Notifications
                .IgnoreQueryFilters()
                .Where(n => userIds.Contains(n.UserId))
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.Notifications
                .IgnoreQueryFilters()
                .Where(n => n.Booking != null &&
                    (userIds.Contains(n.Booking.StudentId) ||
                     userIds.Contains(n.Booking.TimeSlot.TeacherId)))
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.Bookings
                .IgnoreQueryFilters()
                .Where(b =>
                    userIds.Contains(b.StudentId) ||
                    userIds.Contains(b.TimeSlot.TeacherId))
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.TimeSlots
                .IgnoreQueryFilters()
                .Where(ts => userIds.Contains(ts.TeacherId))
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.TeacherSettings
                .IgnoreQueryFilters()
                .Where(ts => userIds.Contains(ts.TeacherId))
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.PasswordResetTokens
                .IgnoreQueryFilters()
                .Where(t => userIds.Contains(t.UserId))
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.Users
                .IgnoreQueryFilters()
                .Where(u => u.CreatedByAdminId != null && userIds.Contains(u.CreatedByAdminId.Value))
                .ExecuteUpdateAsync(
                    s => s.SetProperty(u => u.CreatedByAdminId, (Guid?)null),
                    cancellationToken);

            await dbContext.Users
                .IgnoreQueryFilters()
                .Where(u => userIds.Contains(u.Id))
                .ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
