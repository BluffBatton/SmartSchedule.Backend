using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Departments.GetAll;
using SmartSchedule.Application.Services.Departments.GetById;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public DepartmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] string? search,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetDepartmentsQuery(search), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDepartmentById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetDepartmentByIdQuery(id), cancellationToken);
            return Ok(result);
        }
    }
}
