using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface ISliderRepository
    {
        Task<Slider?> GetByIdAsync(int id);
        Task<IEnumerable<Slider>> GetAllAsync();
        Task<IEnumerable<Slider>> GetBySliderTypeAsync(SliderType sliderType);
        Task<IEnumerable<Slider>> GetActiveSlidersByTypeAsync(SliderType sliderType);
        Task<IEnumerable<Slider>> GetSlidersByLocationAsync(string targetLocation);
        Task<IEnumerable<Slider>> GetActiveSlidersByLocationAsync(string targetLocation);
        Task<IEnumerable<Slider>> GetActiveSlidersInDateRangeAsync(SliderType? sliderType = null);
        Task<Slider> CreateAsync(Slider slider);
        Task<Slider> UpdateAsync(Slider slider);
        Task DeleteAsync(int id);
        Task<int> GetMaxOrderAsync(SliderType sliderType);
        Task ReorderSlidersAsync(IEnumerable<(int Id, int Order)> sliderOrders);
        Task IncrementClickCountAsync(int id);
        Task IncrementViewCountAsync(int id);
        Task<IEnumerable<Slider>> GetExpiringSliders(int daysBeforeExpiry);
    }
}