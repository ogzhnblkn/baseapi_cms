using BaseApi.Application.DTOs.User;
using BaseApi.Application.Exceptions;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user?.Data == null)
                throw new NotFoundException($"User with ID {request.Id} not found");

            // Check if email is already taken by another user
            if (user.Data.Email != request.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Data.Id != user.Data.Id)
                    throw new InvalidOperationException("Email is already taken by another user");
            }

            user.Data.Email = request.Email;
            user.Data.FirstName = request.FirstName;
            user.Data.LastName = request.LastName;
            user.Data.IsActive = request.IsActive;

            var updatedUser = await _userRepository.UpdateAsync(user.Data);

            return new UserDto
            {
                Id = updatedUser.Data.Id,
                Username = updatedUser.Data.Username,
                Email = updatedUser.Data.Email,
                FirstName = updatedUser.Data.FirstName,
                LastName = updatedUser.Data.LastName,
                IsActive = updatedUser.Data.IsActive,
                CreatedAt = updatedUser.Data.CreatedAt,
                LastLoginAt = updatedUser.Data.LastLoginAt
            };
        }
    }
}