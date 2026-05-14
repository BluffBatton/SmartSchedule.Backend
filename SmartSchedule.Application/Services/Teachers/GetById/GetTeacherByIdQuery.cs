using MediatR;

namespace SmartSchedule.Application.Services.Teachers.GetById
{
    public sealed record GetTeacherByIdQuery(Guid TeacherId) : IRequest<TeacherDetailsResponse>;
}