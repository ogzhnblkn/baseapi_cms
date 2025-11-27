using BaseApi.Application.DTOs.Menu;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Menus.Queries.GetMenuTree
{
    public class GetMenuTreeQueryHandler : IRequestHandler<GetMenuTreeQuery, MenuTreeDto>
    {
        private readonly IMenuRepository _menuRepository;

        public GetMenuTreeQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuTreeDto> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Menu> rootMenus;

            if (request.ActiveOnly)
            {
                var rootMenusResult = await _menuRepository.GetRootMenusAsync(request.MenuType);

                // If the result failed, return empty tree
                if (!rootMenusResult.Success)
                {
                    return new MenuTreeDto
                    {
                        MenuType = request.MenuType,
                        MenuTypeName = request.MenuType.ToString(),
                        Menus = new List<MenuDto>()
                    };
                }

                rootMenus = rootMenusResult.Data ?? Enumerable.Empty<Domain.Entities.Menu>();
            }
            else
            {
                var allMenusResult = await _menuRepository.GetByMenuTypeAsync(request.MenuType);

                // If the result failed, return empty tree
                if (!allMenusResult.Success)
                {
                    return new MenuTreeDto
                    {
                        MenuType = request.MenuType,
                        MenuTypeName = request.MenuType.ToString(),
                        Menus = new List<MenuDto>()
                    };
                }

                var allMenus = allMenusResult.Data ?? Enumerable.Empty<Domain.Entities.Menu>();
                rootMenus = allMenus.Where(m => m.ParentId == null);
            }

            var menuTree = new MenuTreeDto
            {
                MenuType = request.MenuType,
                MenuTypeName = request.MenuType.ToString(),
                Menus = rootMenus.Select(menu => new MenuDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Slug = menu.Slug,
                    Description = menu.Description,
                    Url = menu.Url,
                    Icon = menu.Icon,
                    ImageUrl = menu.ImageUrl,
                    ParentId = menu.ParentId,
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
                    SubMenus = menu.SubMenus.Where(s => !request.ActiveOnly || s.IsActive).Select(s => new MenuDto
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
                }).OrderBy(m => m.Order).ToList()
            };

            return menuTree;
        }
    }
}