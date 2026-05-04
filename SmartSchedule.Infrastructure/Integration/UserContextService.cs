using Microsoft.AspNetCore.Http;
using SmartSchedule.Application.Interfaces;
using System.Security.Claims;

namespace SmartSchedule.Infrastructure.Integration
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentUserId()
        {
            var userIdValue = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdValue, out var userId))
                return userId;

            return null;
        }

        public string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Role)?.Value;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
