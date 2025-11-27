using BaseApi.Application.DTOs.User;
using MediatR;

namespace BaseApi.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }
}