using BaseApi.Application.DTOs.User;
using MediatR;

namespace BaseApi.Application.Features.Users.Queries.GetUser
{
    public class GetUserQuery : IRequest<UserDto?>
    {
        public int Id { get; set; }
    }
}