using BaseApi.Application.DTOs.Menu;
using MediatR;

namespace BaseApi.Application.Features.Menus.Queries.GetMenu
{
    public class GetMenuQuery : IRequest<MenuDto?>
    {
        public int Id { get; set; }
    }
}