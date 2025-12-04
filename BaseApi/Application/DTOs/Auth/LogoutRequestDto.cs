namespace BaseApi.Application.DTOs.Auth
{
    public class LogoutRequestDto
    {
        public bool LogoutFromAllDevices { get; set; } = false;
    }
}