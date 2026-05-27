using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Bookings.CancelBooking;
using SmartSchedule.Application.Services.Bookings.Create;
using SmartSchedule.Application.Services.Bookings.DeleteBooking;
using SmartSchedule.Application.Services.Bookings.GetById;
using SmartSchedule.Application.Services.Bookings.GetStudentBookings;
using SmartSchedule.Application.Services.Bookings.GetTeacherBookings;

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

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = bookingId },
                new { bookingId });
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

        [HttpGet("teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherBookings(
            [FromQuery] string? status,
            [FromQuery] string? scope,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetTeacherBookingsQuery(status, scope),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetBookingByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(
            Guid id,
            [FromBody] CancelBookingRequest? request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new CancelBookingCommand(id, request?.CancelReason),
                cancellationToken);

            return Ok(new
            {
                message = "Booking has been cancelled successfully."
            });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteBookingCommand(id), cancellationToken);

            return Ok(new
            {
                message = "Booking has been deleted successfully."
            });
        }
    }

    public sealed class CancelBookingRequest
    {
        public string? CancelReason { get; set; }
    }
}
