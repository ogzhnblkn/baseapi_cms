using BaseApi.Application.DTOs.Menu;
using BaseApi.Application.Exceptions;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Menus.Commands.UpdateMenu
{
    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
    {
        private readonly IMenuRepository _menuRepository;

        public UpdateMenuCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _menuRepository.GetByIdAsync(request.Id);
            if (menu == null)
                throw new NotFoundException($"Menu with ID {request.Id} not found");

            // Validate parent menu if ParentId is provided
            if (request.ParentId.HasValue)
            {
                if (request.ParentId.Value == request.Id)
                    throw new InvalidOperationException("Menu cannot be its own parent");

                var parent = await _menuRepository.GetByIdAsync(request.ParentId.Value);
                if (parent == null)
                    throw new InvalidOperationException("Parent menu not found");

                if (parent.ParentId != null)
                    throw new InvalidOperationException("Cannot move menu under a sub-menu. Maximum 2 levels allowed.");

                // Check if trying to make a parent menu a child of its own child
                var subMenus = await _menuRepository.GetSubMenusAsync(request.Id);
                if (subMenus.Any(s => s.Id == request.ParentId.Value))
                    throw new InvalidOperationException("Cannot move menu under its own sub-menu");
            }

            // Check slug uniqueness
            if (!string.IsNullOrEmpty(request.Slug) && request.Slug != menu.Slug)
            {
                if (await _menuRepository.ExistsAsync(request.Slug, request.Id))
                    throw new InvalidOperationException("Slug already exists");
            }

            menu.Name = request.Name;
            menu.Slug = request.Slug ?? menu.Slug;
            menu.Description = request.Description;
            menu.Url = request.Url;
            menu.Icon = request.Icon;
            menu.ImageUrl = request.ImageUrl;
            menu.ParentId = request.ParentId;
            menu.Order = request.Order;
            menu.IsActive = request.IsActive;
            menu.MenuType = request.MenuType;
            menu.LinkType = request.LinkType;
            menu.OpenInNewTab = request.OpenInNewTab;
            menu.ShowForGuests = request.ShowForGuests;
            menu.ShowForAuthenticated = request.ShowForAuthenticated;

            var updatedMenu = await _menuRepository.UpdateAsync(menu);

            return new MenuDto
            {
                Id = updatedMenu.Id,
                Name = updatedMenu.Name,
                Slug = updatedMenu.Slug,
                Description = updatedMenu.Description,
                Url = updatedMenu.Url,
                Icon = updatedMenu.Icon,
                ImageUrl = updatedMenu.ImageUrl,
                ParentId = updatedMenu.ParentId,
                ParentName = updatedMenu.Parent?.Name,
                Order = updatedMenu.Order,
                IsActive = updatedMenu.IsActive,
                MenuType = updatedMenu.MenuType,
                MenuTypeName = updatedMenu.MenuType.ToString(),
                LinkType = updatedMenu.LinkType,
                LinkTypeName = updatedMenu.LinkType.ToString(),
                OpenInNewTab = updatedMenu.OpenInNewTab,
                ShowForGuests = updatedMenu.ShowForGuests,
                ShowForAuthenticated = updatedMenu.ShowForAuthenticated,
                CreatedAt = updatedMenu.CreatedAt,
                UpdatedAt = updatedMenu.UpdatedAt
            };
        }
    }
}