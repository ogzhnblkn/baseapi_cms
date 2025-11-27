using BaseApi.Application.DTOs.Menu;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Menus.Queries.GetMenuTree
{
    public class GetMenuTreeQuery : IRequest<MenuTreeDto>
    {
        public MenuType MenuType { get; set; }
        public bool ActiveOnly { get; set; } = true;
    }
}