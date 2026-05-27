using MediatR;

namespace SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeachers
{
    public sealed record GetAdminTeachersQuery(
        string? Search,
        Guid? DepartmentId,
        string? Status
    ) : IRequest<List<AdminTeacherResponse>>;
}
