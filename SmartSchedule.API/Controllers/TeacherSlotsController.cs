using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.TeacherSlots.CreateSlot;
using SmartSchedule.Application.Services.TeacherSlots.DeleteSlot;
using SmartSchedule.Application.Services.TeacherSlots.GetMySlots;
using SmartSchedule.Application.Services.TeacherSlots.UpdateSlot;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/teachers/me/slots")]
    [Authorize(Roles = "Teacher")]
    public class TeacherSlotsController : ControllerBase
    {
        private readonly ISender _sender;

        public TeacherSlotsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySlots(
            [FromQuery] string? status,
            [FromQuery] string? scope,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetMyTeacherSlotsQuery(status, scope),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot(
            [FromBody] CreateTeacherSlotCommand command,
            CancellationToken cancellationToken)
        {
            var slotId = await _sender.Send(command, cancellationToken);
            return Created($"/api/teachers/me/slots/{slotId}", new { slotId });
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateSlot(
            Guid id,
            [FromBody] UpdateTeacherSlotRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdateTeacherSlotCommand
                {
                    SlotId = id,
                    StartAtUtc = request.StartAtUtc,
                    EndAtUtc = request.EndAtUtc
                },
                cancellationToken);

            return Ok(new { message = "Slot has been updated successfully." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSlot(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteTeacherSlotCommand(id), cancellationToken);
            return Ok(new { message = "Slot has been deleted successfully." });
        }
    }

    public sealed class UpdateTeacherSlotRequest
    {
        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }
    }
}
