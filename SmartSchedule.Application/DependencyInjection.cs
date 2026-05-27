using Microsoft.Extensions.DependencyInjection;
using SmartSchedule.Application.Common.Configuration;
using System.Reflection;

namespace SmartSchedule.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddOptions<BookingRulesOptions>();

            return services;
        }
    }
}
