using BaseApi.Application.Features.Pages.Commands.CreatePage;
using BaseApi.Application.Features.Pages.Commands.DeletePage;
using BaseApi.Application.Features.Pages.Commands.UpdatePage;
using BaseApi.Application.Features.Pages.Queries.GetPageById;
using BaseApi.Application.Features.Pages.Queries.GetPages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> GetPages([FromQuery] GetPagesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPageById(int id)
        {
            var result = await _mediator.Send(new GetPageByIdQuery { Id = id });

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePage([FromBody] CreatePageCommand command)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized(new { message = "User not authenticated" });

            command.CreatedBy = userId;

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetPageById), new { id = result.Id }, result);
            }

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePage(int id, [FromBody] UpdatePageCommand command)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized(new { message = "User not authenticated" });

            command.Id = id;
            command.UpdatedBy = userId;

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePage(int id)
        {
            var result = await _mediator.Send(new DeletePageCommand { Id = id });

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
    }
}