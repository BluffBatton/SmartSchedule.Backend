using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Teachers.GetById
{
    internal sealed class GetTeacherByIdQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetTeacherByIdQuery, TeacherDetailsResponse>
    {
        public async Task<TeacherDetailsResponse> Handle(
            GetTeacherByIdQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var teacher = await context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id == request.TeacherId &&
                    u.Role == UserRole.Teacher &&
                    u.Status == UserStatus.Active)
                .Select(u => new TeacherDetailsResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    AvailableSlotsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Status == TimeSlotStatus.Available &&
                        ts.StartAtUtc > now)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (teacher is null)
            {
                throw new NotFoundException("Teacher not found.");
            }

            return teacher;
        }
    }
}