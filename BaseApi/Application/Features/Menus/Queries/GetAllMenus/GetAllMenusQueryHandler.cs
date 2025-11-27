using BaseApi.Application.DTOs.Menu;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Menus.Queries.GetAllMenus
{
    public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, IEnumerable<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetAllMenusQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Menu> menus;

            if (request.MenuType.HasValue && request.IsActive == true)
            {
                menus = _menuRepository.GetActiveMenusByTypeAsync(request.MenuType.Value).Result?.Data;
            }
            else if (request.MenuType.HasValue)
            {
                menus = _menuRepository.GetByMenuTypeAsync(request.MenuType.Value).Result?.Data;
            }
            else
            {
                menus = _menuRepository.GetAllAsync().Result?.Data;
            }

            if (request.IsActive.HasValue)
            {
                menus = menus.Where(m => m.IsActive == request.IsActive.Value);
            }

            return menus.Select(menu => new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Slug = menu.Slug,
                Description = menu.Description,
                Url = menu.Url,
                Icon = menu.Icon,
                ImageUrl = menu.ImageUrl,
                ParentId = menu.ParentId,
                ParentName = menu.Parent?.Name,
                Order = menu.Order,
                IsActive = menu.IsActive,
                MenuType = menu.MenuType,
                MenuTypeName = menu.MenuType.ToString(),
                LinkType = menu.LinkType,
                LinkTypeName = menu.LinkType.ToString(),
                OpenInNewTab = menu.OpenInNewTab,
                ShowForGuests = menu.ShowForGuests,
                ShowForAuthenticated = menu.ShowForAuthenticated,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.UpdatedAt
            });
        }
    }
}