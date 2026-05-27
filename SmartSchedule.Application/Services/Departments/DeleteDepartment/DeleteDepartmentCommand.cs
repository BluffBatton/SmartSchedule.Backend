using MediatR;

namespace SmartSchedule.Application.Services.Departments.DeleteDepartment
{
    public sealed record DeleteDepartmentCommand(Guid DepartmentId) : IRequest;
}
