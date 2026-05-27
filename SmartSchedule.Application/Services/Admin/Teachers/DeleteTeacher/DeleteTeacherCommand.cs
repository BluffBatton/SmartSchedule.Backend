using MediatR;

namespace SmartSchedule.Application.Services.Admin.Teachers.DeleteTeacher
{
    public sealed record DeleteTeacherCommand(Guid TeacherId) : IRequest;
}
