using MediatR;

namespace BaseApi.Application.Features.Menus.Commands.DeleteMenu
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}