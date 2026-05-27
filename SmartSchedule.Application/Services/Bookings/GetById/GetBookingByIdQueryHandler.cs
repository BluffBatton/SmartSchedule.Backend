using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Bookings.GetById
{
    internal sealed class GetBookingByIdQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetBookingByIdQuery, BookingDetailsResponse>
    {
        public async Task<BookingDetailsResponse> Handle(
            GetBookingByIdQuery request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var userRole = userContextService.GetCurrentUserRole();
            var isAdmin = userRole == UserRole.Admin.ToString();

            var booking = await context.Bookings
                .AsNoTracking()
                .Where(b => b.Id == request.BookingId)
                .Select(b => new
                {
                    Booking = new BookingDetailsResponse
                    {
                        Id = b.Id,
                        TimeSlotId = b.TimeSlotId,
                        StudentId = b.StudentId,
                        StudentFullName = b.Student.FirstName + " " + b.Student.LastName,
                        StudentEmail = b.Student.Email,
                        TeacherId = b.TimeSlot.TeacherId,
                        TeacherFullName = b.TimeSlot.Teacher.FirstName + " " + b.TimeSlot.Teacher.LastName,
                        TeacherEmail = b.TimeSlot.Teacher.Email,
                        DepartmentName = b.TimeSlot.Teacher.Department != null
                            ? b.TimeSlot.Teacher.Department.Name
                            : null,
                        StartAtUtc = b.TimeSlot.StartAtUtc,
                        EndAtUtc = b.TimeSlot.EndAtUtc,
                        Status = b.Status.ToString(),
                        BookedAtUtc = b.BookedAtUtc,
                        CancelledAtUtc = b.CancelledAtUtc,
                        CancelReason = b.CancelReason
                    },
                    StudentOwnerId = b.StudentId,
                    TeacherOwnerId = b.TimeSlot.TeacherId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
            {
                throw new NotFoundException(
                    $"Booking with id '{request.BookingId}' was not found.");
            }

            if (!isAdmin &&
                booking.StudentOwnerId != userId &&
                booking.TeacherOwnerId != userId)
            {
                throw new ForbiddenException("You are not allowed to access this booking.");
            }

            return booking.Booking;
        }
    }
}
