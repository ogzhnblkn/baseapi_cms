using BaseApi.Application.DTOs.Auth;
using BaseApi.Application.DTOs.User;
using BaseApi.Application.Interfaces;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Data.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (!user.Data.IsActive)
            {
                throw new UnauthorizedAccessException("User account is deactivated");
            }

            // Update last login
            user.Data.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user.Data);

            var token = _jwtService.GenerateToken(user.Data);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserDto
                {
                    Id = user.Data.Id,
                    Username = user.Data.Username,
                    Email = user.Data.Email,
                    FirstName = user.Data.FirstName,
                    LastName = user.Data.LastName,
                    IsActive = user.Data.IsActive,
                    CreatedAt = user.Data.CreatedAt,
                    LastLoginAt = user.Data.LastLoginAt
                }
            };
        }
    }
}