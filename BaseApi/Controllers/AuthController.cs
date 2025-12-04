using BaseApi.Application.DTOs.Auth;
using BaseApi.Application.Features.Auth.Commands.Login;
using BaseApi.Application.Features.Auth.Commands.Logout;
using BaseApi.Application.Features.Auth.Commands.Register;
using BaseApi.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;

        public AuthController(IMediator mediator, IJwtService jwtService)
        {
            _mediator = mediator;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var command = new LoginCommand
                {
                    Username = request.Username,
                    Password = request.Password
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var command = new RegisterCommand
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                var result = await _mediator.Send(command);
                return Ok(result);
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

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] LogoutRequestDto? request = null)
        {
            try
            {
                var authHeader = Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return BadRequest(new { message = "Authorization header is missing" });
                }

                var token = _jwtService.GetTokenFromAuthorizationHeader(authHeader);
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var command = new LogoutCommand
                {
                    Token = token,
                    UserId = userId,
                    LogoutFromAllDevices = request?.LogoutFromAllDevices ?? false
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<ActionResult> LogoutFromAllDevices()
        {
            try
            {
                var authHeader = Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return BadRequest(new { message = "Authorization header is missing" });
                }

                var token = _jwtService.GetTokenFromAuthorizationHeader(authHeader);
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var command = new LogoutCommand
                {
                    Token = token,
                    UserId = userId,
                    LogoutFromAllDevices = true
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}