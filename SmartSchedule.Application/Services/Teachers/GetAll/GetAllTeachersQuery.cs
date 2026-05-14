using MediatR;

namespace SmartSchedule.Application.Services.Teachers.GetAll
{
    public sealed record GetTeachersQuery(
        string? Search,
        Guid? DepartmentId
    ) : IRequest<List<TeacherResponse>>;
}
