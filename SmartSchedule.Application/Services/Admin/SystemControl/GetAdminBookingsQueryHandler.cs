using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Admin.SystemControl
{
    internal sealed class GetAdminBookingsQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetAdminBookingsQuery, List<AdminBookingResponse>>
    {
        public async Task<List<AdminBookingResponse>> Handle(
            GetAdminBookingsQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.Bookings
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status)
                && Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            {
                query = query.Where(b => b.Status == status);
            }

            return await query
                .OrderByDescending(b => b.BookedAtUtc)
                .Select(b => new AdminBookingResponse
                {
                    Id = b.Id,
                    StudentFullName = b.Student.FirstName + " " + b.Student.LastName,
                    TeacherFullName = b.TimeSlot.Teacher.FirstName + " " + b.TimeSlot.Teacher.LastName,
                    DepartmentName = b.TimeSlot.Teacher.Department != null
                        ? b.TimeSlot.Teacher.Department.Name
                        : null,
                    StartAtUtc = b.TimeSlot.StartAtUtc,
                    EndAtUtc = b.TimeSlot.EndAtUtc,
                    Status = b.Status.ToString(),
                    BookedAtUtc = b.BookedAtUtc
                })
                .ToListAsync(cancellationToken);
        }
    }
}
