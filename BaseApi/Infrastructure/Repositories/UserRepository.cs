using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<User?>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                    return ApiResult<User?>.NotFoundResult(Messages.User.NotFound);

                return ApiResult<User?>.SuccessResult(user, Messages.User.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<User?>.FailureResult($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ApiResult<User?>> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

                if (user == null)
                    return ApiResult<User?>.NotFoundResult(Messages.User.NotFound);

                return ApiResult<User?>.SuccessResult(user, Messages.User.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<User?>.FailureResult($"Error retrieving user by username: {ex.Message}");
            }
        }

        public async Task<ApiResult<User?>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                    return ApiResult<User?>.NotFoundResult(Messages.User.NotFound);

                return ApiResult<User?>.SuccessResult(user, Messages.User.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<User?>.FailureResult($"Error retrieving user by email: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<User>>> GetAllAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                return ApiResult<IEnumerable<User>>.SuccessResult(users, Messages.User.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<User>>.FailureResult($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<ApiResult<User>> CreateAsync(User user)
        {
            try
            {
                // Check if user already exists
                if (await ExistsAsync(user.Username, user.Email))
                {
                    return ApiResult<User>.FailureResult(Messages.User.UserExists);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return ApiResult<User>.SuccessResult(user, Messages.User.Created);
            }
            catch (Exception ex)
            {
                return ApiResult<User>.FailureResult($"Error creating user: {ex.Message}");
            }
        }

        public async Task<ApiResult<User>> UpdateAsync(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return ApiResult<User>.SuccessResult(user, Messages.User.Updated);
            }
            catch (Exception ex)
            {
                return ApiResult<User>.FailureResult($"Error updating user: {ex.Message}");
            }
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return ApiResult.FailureResult(Messages.User.NotFound);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult(Messages.User.Deleted);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Username.ToLower() == username.ToLower() ||
                              u.Email.ToLower() == email.ToLower());
        }
    }
}