using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Infrastructure.Persistance.Seeding
{
    public sealed class DatabaseInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DatabaseInitializerOptions _options;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(
            IServiceProvider serviceProvider,
            IOptions<DatabaseInitializerOptions> options,
            ILogger<DatabaseInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (_options.ApplyMigrationsOnStartup)
            {
                try
                {
                    var pending = await dbContext.Database
                        .GetPendingMigrationsAsync(cancellationToken);

                    if (pending.Any())
                    {
                        _logger.LogInformation(
                            "Applying {Count} pending database migrations.",
                            pending.Count());

                        await dbContext.Database.MigrateAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to apply database migrations.");
                    throw;
                }
            }

            if (_options.SeedAdmin.Enabled)
            {
                await SeedAdminAsync(scope.ServiceProvider, dbContext, cancellationToken);
            }
        }

        private async Task SeedAdminAsync(
            IServiceProvider scopedServices,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var seed = _options.SeedAdmin;
            var email = (seed.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("SeedAdmin email is empty; skipping admin seed.");
                return;
            }

            if (string.IsNullOrWhiteSpace(seed.Password))
            {
                _logger.LogWarning("SeedAdmin password is empty; skipping admin seed.");
                return;
            }

            var adminExists = await dbContext.Users
                .AnyAsync(u => u.Role == UserRole.Admin, cancellationToken);

            if (adminExists)
            {
                _logger.LogDebug("At least one Admin user already exists; skipping admin seed.");
                return;
            }

            var emailTaken = await dbContext.Users
                .AnyAsync(u => u.Email == email, cancellationToken);

            if (emailTaken)
            {
                _logger.LogWarning(
                    "Cannot seed admin: email {Email} is already taken by a non-admin user.",
                    email);
                return;
            }

            var passwordHasher = scopedServices.GetRequiredService<IPasswordHasherService>();

            var admin = new User
            {
                Id = Guid.NewGuid(),
                FirstName = (seed.FirstName ?? "System").Trim(),
                LastName = (seed.LastName ?? "Admin").Trim(),
                Email = email,
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, seed.Password);

            await dbContext.Users.AddAsync(admin, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Seeded initial admin user with email {Email}. Please change the password after first login.",
                email);
        }
    }
}
