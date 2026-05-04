using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Infrastructure.Integration;
using System.Text;

namespace SmartSchedule.Infrastructure.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            return services;
        }
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddHttpContextAccessor();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IUserContextService, UserContextService>();

            var issuer = configuration["Jwt:Issuer"]
                         ?? throw new InvalidOperationException("Jwt:Issuer is missing");
            var audience = configuration["Jwt:Audience"]
                           ?? throw new InvalidOperationException("Jwt:Audience is missing");
            var key = configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("Jwt:Key is missing");



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}