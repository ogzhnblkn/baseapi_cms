using BaseApi.Application.DTOs.User;
using BaseApi.Application.Exceptions;
using BaseApi.Application.Features.Users.Commands.CreateUser;
using BaseApi.Application.Features.Users.Commands.DeleteUser;
using BaseApi.Application.Features.Users.Commands.UpdateUser;
using BaseApi.Application.Features.Users.Queries.GetAllUsers;
using BaseApi.Application.Features.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var result = await _mediator.Send(new GetAllUsersQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetUserQuery { Id = id });

                if (result == null)
                    return NotFound(new { message = $"User with ID {id} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var command = new CreateUserCommand
                {
                    Username = createUserDto.Username,
                    Email = createUserDto.Email,
                    Password = createUserDto.Password,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    IsActive = createUserDto.IsActive
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
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
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var command = new UpdateUserCommand
                {
                    Id = id,
                    Email = updateUserDto.Email,
                    FirstName = updateUserDto.FirstName,
                    LastName = updateUserDto.LastName,
                    IsActive = updateUserDto.IsActive
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
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteUserCommand { Id = id });

                if (!result)
                    return NotFound(new { message = $"User with ID {id} not found" });

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}