using MediatR;

namespace SmartSchedule.Application.Services.Departments.UpdateDepartment
{
    public sealed record UpdateDepartmentCommand(Guid DepartmentId, string Name) : IRequest;
}
