using BaseApi.Application.Common;
using BaseApi.Application.Interfaces;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Auth.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, LogoutResponse>
    {
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LogoutHandler> _logger;

        public LogoutHandler(
            ITokenBlacklistRepository tokenBlacklistRepository,
            IJwtService jwtService,
            IUserRepository userRepository,
            ILogger<LogoutHandler> logger)
        {
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    return new LogoutResponse
                    {
                        Success = false,
                        Message = "Token is required for logout"
                    };
                }

                // Validate token format
                if (!_jwtService.ValidateToken(request.Token))
                {
                    return new LogoutResponse
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                // Get token expiration date
                var tokenExpirationDate = _jwtService.GetTokenExpirationDate(request.Token);

                if (tokenExpirationDate <= DateTime.UtcNow)
                {
                    return new LogoutResponse
                    {
                        Success = true,
                        Message = "Token already expired"
                    };
                }

                // Blacklist the token
                await _tokenBlacklistRepository.BlacklistTokenAsync(
                    request.Token,
                    request.UserId,
                    tokenExpirationDate,
                    "User logout");

                // If logout from all devices is requested
                if (request.LogoutFromAllDevices)
                {
                    await _tokenBlacklistRepository.BlacklistAllUserTokensAsync(request.UserId);
                    _logger.LogInformation("User {UserId} logged out from all devices", request.UserId);
                }
                else
                {
                    _logger.LogInformation("User {UserId} logged out", request.UserId);
                }

                return new LogoutResponse
                {
                    Success = true,
                    Message = Messages.Auth.LogoutSuccess
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", request.UserId);
                return new LogoutResponse
                {
                    Success = false,
                    Message = Messages.General.Error
                };
            }
        }
    }
}