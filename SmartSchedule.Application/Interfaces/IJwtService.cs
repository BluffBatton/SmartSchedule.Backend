using SmartSchedule.Domain.Entities;

namespace SmartSchedule.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
    }
}
