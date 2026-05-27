using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartSchedule.Application.Common.Configuration;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Bookings.Create
{
    internal sealed class CreateBookingCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService,
        IOptionsSnapshot<BookingRulesOptions> bookingRulesSnapshot)
        : IRequestHandler<CreateBookingCommand, Guid>
    {
        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var studentId = userContextService.GetCurrentUserId();

            if (studentId is null)
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            var student = await context.Users
                .FirstOrDefaultAsync(u => u.Id == studentId.Value, cancellationToken);

            if (student is null)
            {
                throw new NotFoundException("Student not found.");
            }

            if (student.Role != UserRole.Student)
            {
                throw new ForbiddenException("Only students can create bookings.");
            }

            if (student.Status == UserStatus.Blocked)
            {
                throw new ForbiddenException("User is blocked.");
            }

            var now = DateTime.UtcNow;

            var timeSlot = await context.TimeSlots
                .Include(ts => ts.Teacher)
                .FirstOrDefaultAsync(ts => ts.Id == request.TimeSlotId, cancellationToken);

            if (timeSlot is null)
            {
                throw new NotFoundException("Time slot not found.");
            }

            if (timeSlot.Teacher.Status != UserStatus.Active)
            {
                throw new ConflictException("Teacher is not active.");
            }

            if (timeSlot.StartAtUtc <= now)
            {
                throw new ConflictException("Cannot book a past time slot.");
            }

            if (timeSlot.Status != TimeSlotStatus.Available)
            {
                throw new ConflictException("Time slot is not available.");
            }

            var rules = bookingRulesSnapshot.Value;
            var maxActive = Math.Max(1, rules.MaxActiveBookingsPerStudent);

            var activeFutureCount = await context.Bookings
                .CountAsync(b =>
                    b.StudentId == studentId.Value &&
                    b.Status == BookingStatus.Active &&
                    b.TimeSlot.StartAtUtc > now,
                    cancellationToken);

            if (activeFutureCount >= maxActive)
            {
                throw new ConflictException(
                    $"You have reached the limit of {maxActive} active bookings. " +
                    "Cancel an existing booking before creating a new one.");
            }

            var alreadyHasActiveWithTeacher = await context.Bookings
                .AnyAsync(b =>
                    b.StudentId == studentId.Value &&
                    b.Status == BookingStatus.Active &&
                    b.TimeSlot.TeacherId == timeSlot.TeacherId &&
                    b.TimeSlot.StartAtUtc > now,
                    cancellationToken);

            if (alreadyHasActiveWithTeacher)
            {
                throw new ConflictException(
                    "You already have an active booking with this teacher.");
            }

            var activeBookingExists = await context.Bookings
                .AnyAsync(b =>
                    b.TimeSlotId == timeSlot.Id &&
                    b.Status == BookingStatus.Active,
                    cancellationToken);

            if (activeBookingExists)
            {
                throw new ConflictException("Time slot is already booked.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                StudentId = studentId.Value,
                TimeSlotId = timeSlot.Id,
                Status = BookingStatus.Active,
                BookedAtUtc = now
            };

            timeSlot.Status = TimeSlotStatus.Booked;

            var studentNotification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = studentId.Value,
                BookingId = booking.Id,
                Title = "Booking created",
                Message = "You have successfully booked a consultation.",
                Type = NotificationType.BookingCreated,
                IsRead = false,
                SentAtUtc = now
            };

            var teacherNotification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = timeSlot.TeacherId,
                BookingId = booking.Id,
                Title = "New consultation booking",
                Message = "A student has booked your consultation time slot.",
                Type = NotificationType.BookingCreated,
                IsRead = false,
                SentAtUtc = now
            };

            await context.Bookings.AddAsync(booking, cancellationToken);
            await context.Notifications.AddAsync(studentNotification, cancellationToken);
            await context.Notifications.AddAsync(teacherNotification, cancellationToken);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw new ConflictException(
                    "Time slot was just booked by another student. Please pick another slot.");
            }

            return booking.Id;
        }
    }
}
