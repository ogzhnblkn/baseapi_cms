using BaseApi.Application.DTOs.Menu;
using BaseApi.Application.Exceptions;
using BaseApi.Application.Features.Menus.Commands.CreateMenu;
using BaseApi.Application.Features.Menus.Commands.DeleteMenu;
using BaseApi.Application.Features.Menus.Commands.UpdateMenu;
using BaseApi.Application.Features.Menus.Queries.GetAllMenus;
using BaseApi.Application.Features.Menus.Queries.GetMenu;
using BaseApi.Application.Features.Menus.Queries.GetMenuTree;
using BaseApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetAllMenus(
            [FromQuery] MenuType? menuType = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var query = new GetAllMenusQuery
                {
                    MenuType = menuType,
                    IsActive = isActive
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetMenu(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetMenuQuery { Id = id });

                if (result == null)
                    return NotFound(new { message = $"Menu with ID {id} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("tree/{menuType}")]
        [AllowAnonymous]
        public async Task<ActionResult<MenuTreeDto>> GetMenuTree(
            MenuType menuType,
            [FromQuery] bool activeOnly = true)
        {
            try
            {
                var query = new GetMenuTreeQuery
                {
                    MenuType = menuType,
                    ActiveOnly = activeOnly
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] CreateMenuDto createMenuDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new CreateMenuCommand
                {
                    Name = createMenuDto.Name,
                    Slug = createMenuDto.Slug,
                    Description = createMenuDto.Description,
                    Url = createMenuDto.Url,
                    Icon = createMenuDto.Icon,
                    ImageUrl = createMenuDto.ImageUrl,
                    ParentId = createMenuDto.ParentId,
                    Order = createMenuDto.Order,
                    IsActive = createMenuDto.IsActive,
                    MenuType = createMenuDto.MenuType,
                    LinkType = createMenuDto.LinkType,
                    OpenInNewTab = createMenuDto.OpenInNewTab,
                    ShowForGuests = createMenuDto.ShowForGuests,
                    ShowForAuthenticated = createMenuDto.ShowForAuthenticated,
                    CreatedBy = userId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetMenu), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MenuDto>> UpdateMenu(int id, [FromBody] UpdateMenuDto updateMenuDto)
        {
            try
            {
                var command = new UpdateMenuCommand
                {
                    Id = id,
                    Name = updateMenuDto.Name,
                    Slug = updateMenuDto.Slug,
                    Description = updateMenuDto.Description,
                    Url = updateMenuDto.Url,
                    Icon = updateMenuDto.Icon,
                    ImageUrl = updateMenuDto.ImageUrl,
                    ParentId = updateMenuDto.ParentId,
                    Order = updateMenuDto.Order,
                    IsActive = updateMenuDto.IsActive,
                    MenuType = updateMenuDto.MenuType,
                    LinkType = updateMenuDto.LinkType,
                    OpenInNewTab = updateMenuDto.OpenInNewTab,
                    ShowForGuests = updateMenuDto.ShowForGuests,
                    ShowForAuthenticated = updateMenuDto.ShowForAuthenticated
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMenu(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteMenuCommand { Id = id });

                if (!result)
                    return NotFound(new { message = $"Menu with ID {id} not found" });

                return Ok(new { message = "Menu deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}