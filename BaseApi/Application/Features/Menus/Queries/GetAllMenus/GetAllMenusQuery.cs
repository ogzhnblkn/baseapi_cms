using BaseApi.Application.DTOs.Menu;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Menus.Queries.GetAllMenus
{
    public class GetAllMenusQuery : IRequest<IEnumerable<MenuDto>>
    {
        public MenuType? MenuType { get; set; }
        public bool? IsActive { get; set; }
    }
}