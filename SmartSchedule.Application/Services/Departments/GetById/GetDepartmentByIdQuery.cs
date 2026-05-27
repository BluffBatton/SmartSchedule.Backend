using MediatR;
using SmartSchedule.Application.Services.Departments.GetAll;

namespace SmartSchedule.Application.Services.Departments.GetById
{
    public sealed record GetDepartmentByIdQuery(Guid DepartmentId)
        : IRequest<DepartmentResponse>;
}
