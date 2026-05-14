using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Teachers.GetAll;
using SmartSchedule.Application.Services.Teachers.GetAvailableTeacherSlots;
using SmartSchedule.Application.Services.Teachers.GetById;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ISender _sender;

        public TeachersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachers(
            [FromQuery] string? search,
            [FromQuery] Guid? departmentId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetTeachersQuery(search, departmentId),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTeacherById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetTeacherByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAvailableTeacherSlotsQuery(id),
                cancellationToken);

            return Ok(result);
        }
    }
}
