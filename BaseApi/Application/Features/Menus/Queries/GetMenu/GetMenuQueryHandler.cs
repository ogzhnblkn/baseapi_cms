using BaseApi.Application.DTOs.Menu;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Menus.Queries.GetMenu
{
    public class GetMenuQueryHandler : IRequestHandler<GetMenuQuery, MenuDto?>
    {
        private readonly IMenuRepository _menuRepository;

        public GetMenuQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuDto?> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            var menu = await _menuRepository.GetByIdAsync(request.Id);
            if (menu == null)
                return null;

            return new MenuDto
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
                UpdatedAt = menu.UpdatedAt,
                SubMenus = menu.SubMenus.Select(s => new MenuDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Slug = s.Slug,
                    Description = s.Description,
                    Url = s.Url,
                    Icon = s.Icon,
                    ImageUrl = s.ImageUrl,
                    ParentId = s.ParentId,
                    Order = s.Order,
                    IsActive = s.IsActive,
                    MenuType = s.MenuType,
                    MenuTypeName = s.MenuType.ToString(),
                    LinkType = s.LinkType,
                    LinkTypeName = s.LinkType.ToString(),
                    OpenInNewTab = s.OpenInNewTab,
                    ShowForGuests = s.ShowForGuests,
                    ShowForAuthenticated = s.ShowForAuthenticated,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).OrderBy(s => s.Order).ToList()
            };
        }
    }
}