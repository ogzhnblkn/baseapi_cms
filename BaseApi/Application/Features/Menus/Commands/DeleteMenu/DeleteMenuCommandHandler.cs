using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Menus.Commands.DeleteMenu
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IMenuRepository _menuRepository;

        public DeleteMenuCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _menuRepository.GetByIdAsync(request.Id);
            if (menu == null)
                return false;

            // Check if menu has sub-menus
            var subMenus = await _menuRepository.GetSubMenusAsync(request.Id);
            if (subMenus.Data.Any())
            {
                throw new InvalidOperationException("Cannot delete menu that has sub-menus. Delete sub-menus first.");
            }

            await _menuRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}