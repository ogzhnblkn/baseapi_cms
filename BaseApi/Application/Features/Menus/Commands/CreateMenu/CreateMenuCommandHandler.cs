using BaseApi.Application.DTOs.Menu;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Menus.Commands.CreateMenu
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
    {
        private readonly IMenuRepository _menuRepository;

        public CreateMenuCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            // Validate parent menu exists if ParentId is provided
            if (request.ParentId.HasValue)
            {
                var parent = await _menuRepository.GetByIdAsync(request.ParentId.Value);
                if (parent == null)
                    throw new InvalidOperationException("Parent menu not found");

                if (parent.ParentId != null)
                    throw new InvalidOperationException("Cannot create menu under a sub-menu. Maximum 2 levels allowed.");
            }

            var menu = new Menu
            {
                Name = request.Name,
                Slug = request.Slug ?? string.Empty,
                Description = request.Description,
                Url = request.Url,
                Icon = request.Icon,
                ImageUrl = request.ImageUrl,
                ParentId = request.ParentId,
                Order = request.Order,
                IsActive = request.IsActive,
                MenuType = request.MenuType,
                LinkType = request.LinkType,
                OpenInNewTab = request.OpenInNewTab,
                ShowForGuests = request.ShowForGuests,
                ShowForAuthenticated = request.ShowForAuthenticated,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdMenu = await _menuRepository.CreateAsync(menu);

            return new MenuDto
            {
                Id = createdMenu.Id,
                Name = createdMenu.Name,
                Slug = createdMenu.Slug,
                Description = createdMenu.Description,
                Url = createdMenu.Url,
                Icon = createdMenu.Icon,
                ImageUrl = createdMenu.ImageUrl,
                ParentId = createdMenu.ParentId,
                Order = createdMenu.Order,
                IsActive = createdMenu.IsActive,
                MenuType = createdMenu.MenuType,
                MenuTypeName = createdMenu.MenuType.ToString(),
                LinkType = createdMenu.LinkType,
                LinkTypeName = createdMenu.LinkType.ToString(),
                OpenInNewTab = createdMenu.OpenInNewTab,
                ShowForGuests = createdMenu.ShowForGuests,
                ShowForAuthenticated = createdMenu.ShowForAuthenticated,
                CreatedAt = createdMenu.CreatedAt,
                UpdatedAt = createdMenu.UpdatedAt
            };
        }
    }
}