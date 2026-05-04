using SmartSchedule.Domain.Entities;

namespace SmartSchedule.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string password, string passwordHash);
    }
}
