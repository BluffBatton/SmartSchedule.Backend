using MediatR;

namespace SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeacherById
{
    public sealed record GetAdminTeacherByIdQuery(Guid TeacherId)
        : IRequest<AdminTeacherDetailsResponse>;
}
