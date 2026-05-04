using SmartSchedule.Domain.Common;

namespace SmartSchedule.Domain.Entities
{
    public class TeacherSetting : BaseEntity
    {
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public string WorkDays { get; set; } = string.Empty;

        public TimeOnly DefaultStartTime { get; set; }

        public TimeOnly DefaultEndTime { get; set; }

        public int ConsultationDurationMinutes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
