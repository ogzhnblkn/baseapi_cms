using MediatR;

namespace BaseApi.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<LogoutResponse>
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool LogoutFromAllDevices { get; set; } = false;
    }
}