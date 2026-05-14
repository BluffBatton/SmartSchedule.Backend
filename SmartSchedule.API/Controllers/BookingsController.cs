using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Bookings.CancelBooking;
using SmartSchedule.Application.Services.Bookings.Create;
using SmartSchedule.Application.Services.Bookings.GetStudentBookings;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly ISender _sender;

        public BookingsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateBooking(
            [FromBody] CreateBookingCommand command,
            CancellationToken cancellationToken)
        {
            var bookingId = await _sender.Send(command, cancellationToken);

            return Ok(new
            {
                bookingId
            });
        }

        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetStudentBookingsQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(
            Guid id,
            [FromBody] CancelBookingRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new CancelBookingCommand(id, request.CancelReason),
                cancellationToken);

            return Ok(new
            {
                message = "Booking has been cancelled successfully."
            });
        }
    }

    public sealed class CancelBookingRequest
    {
        public string? CancelReason { get; set; }
    }
}
