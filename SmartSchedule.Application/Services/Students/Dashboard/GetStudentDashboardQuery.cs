using MediatR;

namespace SmartSchedule.Application.Services.Students.Dashboard
{
    public sealed record GetStudentDashboardQuery : IRequest<DashboardResponse>;
}
