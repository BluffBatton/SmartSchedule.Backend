using MediatR;

namespace SmartSchedule.Application.Services.Admin.Teachers.UpdateTeacher
{
    public sealed class UpdateTeacherCommand : IRequest
    {
        public Guid TeacherId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public Guid? DepartmentId { get; set; }

        public string? Status { get; set; }
    }
}
