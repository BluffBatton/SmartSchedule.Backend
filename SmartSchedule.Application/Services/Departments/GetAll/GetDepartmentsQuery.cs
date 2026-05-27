using MediatR;

namespace SmartSchedule.Application.Services.Departments.GetAll
{
    public sealed record GetDepartmentsQuery(string? Search) : IRequest<List<DepartmentResponse>>;
}
