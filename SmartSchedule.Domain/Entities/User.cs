using SmartSchedule.Domain.Common;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Guid? CreatedByAdminId { get; set; }
        public User? CreatedByAdmin { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime? LastLoginAtUtc { get; set; }


        public TeacherSetting? TeacherSetting { get; set; }

        public ICollection<TimeSlot> TeacherTimeSlots { get; set; } = new List<TimeSlot>();

        public ICollection<Booking> StudentBookings { get; set; } = new List<Booking>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

        public ICollection<User> CreatedUsers { get; set; } = new List<User>();
    }
}
