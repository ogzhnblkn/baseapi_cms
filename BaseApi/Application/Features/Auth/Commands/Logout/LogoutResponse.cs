namespace BaseApi.Application.Features.Auth.Commands.Logout
{
    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}