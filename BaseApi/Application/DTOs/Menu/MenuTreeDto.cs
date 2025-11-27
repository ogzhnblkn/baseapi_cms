using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Menu
{
    public class MenuTreeDto
    {
        public MenuType MenuType { get; set; }
        public string MenuTypeName { get; set; } = string.Empty;
        public List<MenuDto> Menus { get; set; } = new();
    }
}