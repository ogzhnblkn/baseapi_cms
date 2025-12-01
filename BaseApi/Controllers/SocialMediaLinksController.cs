using BaseApi.Application.Features.SocialMediaLinks.Commands.CreateSocialMediaLink;
using BaseApi.Application.Features.SocialMediaLinks.Commands.DeleteSocialMediaLink;
using BaseApi.Application.Features.SocialMediaLinks.Commands.UpdateSocialMediaLink;
using BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinkById;
using BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediaLinksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SocialMediaLinksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSocialMediaLinks([FromQuery] GetSocialMediaLinksQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSocialMediaLinkById(int id)
        {
            var result = await _mediator.Send(new GetSocialMediaLinkByIdQuery { Id = id });

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveLinks()
        {
            var result = await _mediator.Send(new GetSocialMediaLinksQuery { ActiveOnly = true });
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSocialMediaLink([FromBody] CreateSocialMediaLinkCommand command)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
                return Unauthorized(new { message = "User not authenticated" });

            command.CreatedBy = userId;

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetSocialMediaLinkById), new { id = result.Id }, result);
            }

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSocialMediaLink(int id, [FromBody] UpdateSocialMediaLinkCommand command)
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
        public async Task<IActionResult> DeleteSocialMediaLink(int id)
        {
            var result = await _mediator.Send(new DeleteSocialMediaLinkCommand { Id = id });

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
    }
}