using MediatR;

namespace SmartSchedule.Application.Services.Departments.CreateDepartment
{
    public sealed record CreateDepartmentCommand(string Name) : IRequest<Guid>;
}
