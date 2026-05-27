using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Admin.Teachers.DeleteTeacher
{
    internal sealed class DeleteTeacherCommandHandler(IApplicationDbContext context)
        : IRequestHandler<DeleteTeacherCommand>
    {
        public async Task Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Users
                .Include(u => u.TeacherSetting)
                .Include(u => u.TeacherTimeSlots)
                .FirstOrDefaultAsync(
                    u => u.Id == request.TeacherId && u.Role == UserRole.Teacher,
                    cancellationToken);

            if (teacher is null)
            {
                throw new NotFoundException($"Teacher with id '{request.TeacherId}' was not found.");
            }

            var now = DateTime.UtcNow;

            var hasFutureActiveBookings = await context.Bookings
                .AnyAsync(b =>
                    b.Status == BookingStatus.Active &&
                    b.TimeSlot.TeacherId == teacher.Id &&
                    b.TimeSlot.StartAtUtc > now,
                    cancellationToken);

            if (hasFutureActiveBookings)
            {
                throw new ConflictException(
                    "Teacher has future active bookings. Cancel them before deleting the teacher.");
            }

            var futureSlots = teacher.TeacherTimeSlots
                .Where(ts => ts.StartAtUtc > now)
                .ToList();

            foreach (var slot in futureSlots)
            {
                context.TimeSlots.Remove(slot);
            }

            if (teacher.TeacherSetting is not null)
            {
                context.TeacherSettings.Remove(teacher.TeacherSetting);
            }

            context.Users.Remove(teacher);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
