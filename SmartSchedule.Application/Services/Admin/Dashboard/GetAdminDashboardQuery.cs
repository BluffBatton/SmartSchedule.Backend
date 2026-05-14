using MediatR;

namespace SmartSchedule.Application.Services.Admin.Dashboard
{
    public sealed record GetAdminDashboardQuery() : IRequest<AdminDashboardResponse>;
}