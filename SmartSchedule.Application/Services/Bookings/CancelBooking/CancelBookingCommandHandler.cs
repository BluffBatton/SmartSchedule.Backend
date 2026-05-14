using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Bookings.CancelBooking
{
    internal sealed class CancelBookingCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<CancelBookingCommand>
    {
        public async Task Handle(
            CancelBookingCommand request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId();

            if (userId is null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userRole = userContextService.GetCurrentUserRole();

            var booking = await context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking is null)
            {
                throw new InvalidOperationException("Booking not found.");
            }

            if (booking.Status != BookingStatus.Active)
            {
                throw new InvalidOperationException("Only active booking can be cancelled.");
            }

            var isStudentOwner = userRole == UserRole.Student.ToString()
                                 && booking.StudentId == userId.Value;

            var isTeacherOwner = userRole == UserRole.Teacher.ToString()
                                 && booking.TimeSlot.TeacherId == userId.Value;

            var isAdmin = userRole == UserRole.Admin.ToString();

            if (!isStudentOwner && !isTeacherOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("You are not allowed to cancel this booking.");
            }

            var now = DateTime.UtcNow;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAtUtc = now;
            booking.CancelReason = request.CancelReason;

            if (booking.TimeSlot.StartAtUtc > now)
            {
                booking.TimeSlot.Status = TimeSlotStatus.Available;
            }
            else
            {
                booking.TimeSlot.Status = TimeSlotStatus.Cancelled;
            }

            var studentNotification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = booking.StudentId,
                BookingId = booking.Id,
                Title = "Booking cancelled",
                Message = "Your consultation booking has been cancelled.",
                Type = NotificationType.BookingCancelled,
                IsRead = false,
                SentAtUtc = now
            };

            var teacherNotification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = booking.TimeSlot.TeacherId,
                BookingId = booking.Id,
                Title = "Consultation cancelled",
                Message = "A consultation booking has been cancelled.",
                Type = NotificationType.BookingCancelled,
                IsRead = false,
                SentAtUtc = now
            };

            await context.Notifications.AddAsync(studentNotification, cancellationToken);
            await context.Notifications.AddAsync(teacherNotification, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
