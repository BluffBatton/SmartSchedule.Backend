using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Application.Services.TeacherSlots.Common;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.TeacherSlots.CreateSlot
{
    internal sealed class CreateTeacherSlotCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<CreateTeacherSlotCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateTeacherSlotCommand request,
            CancellationToken cancellationToken)
        {
            var teacherId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var teacher = await context.Users
                .FirstOrDefaultAsync(u => u.Id == teacherId, cancellationToken)
                ?? throw new NotFoundException("Teacher not found.");

            if (teacher.Role != UserRole.Teacher)
                throw new ForbiddenException("Only teachers can publish consultation slots.");

            if (teacher.Status == UserStatus.Blocked)
                throw new ForbiddenException("Teacher account is blocked.");

            var startUtc = DateTime.SpecifyKind(request.StartAtUtc, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(request.EndAtUtc, DateTimeKind.Utc);

            TeacherSlotValidator.ValidateRange(startUtc, endUtc, DateTime.UtcNow);

            await TeacherSlotValidator.EnsureNoOverlapAsync(
                context,
                teacherId,
                startUtc,
                endUtc,
                excludeSlotId: null,
                cancellationToken);

            var slot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                TeacherId = teacherId,
                StartAtUtc = startUtc,
                EndAtUtc = endUtc,
                Status = TimeSlotStatus.Available
            };

            await context.TimeSlots.AddAsync(slot, cancellationToken);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw new ConflictException(
                    "Slot with the same start and end time already exists for this teacher.");
            }

            return slot.Id;
        }
    }
}
