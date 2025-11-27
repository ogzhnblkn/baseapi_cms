using BaseApi.Application.Common;
using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface ISliderRepository
    {
        Task<ApiResult<Slider?>> GetByIdAsync(int id);
        Task<ApiResult<IEnumerable<Slider>>> GetAllAsync();
        Task<ApiResult<IEnumerable<Slider>>> GetBySliderTypeAsync(SliderType sliderType);
        Task<ApiResult<IEnumerable<Slider>>> GetActiveSlidersByTypeAsync(SliderType sliderType);
        Task<ApiResult<IEnumerable<Slider>>> GetSlidersByLocationAsync(string targetLocation);
        Task<ApiResult<IEnumerable<Slider>>> GetActiveSlidersByLocationAsync(string targetLocation);
        Task<ApiResult<IEnumerable<Slider>>> GetActiveSlidersInDateRangeAsync(SliderType? sliderType = null);
        Task<ApiResult<Slider>> CreateAsync(Slider slider);
        Task<ApiResult<Slider>> UpdateAsync(Slider slider);
        Task<ApiResult> DeleteAsync(int id);
        Task<ApiResult<int>> GetMaxOrderAsync(SliderType sliderType);
        Task<ApiResult> ReorderSlidersAsync(IEnumerable<(int Id, int Order)> sliderOrders);
        Task<ApiResult> IncrementClickCountAsync(int id);
        Task<ApiResult> IncrementViewCountAsync(int id);
        //Task<IEnumerable<Slider>> GetExpiringSlidersAsync(int daysBeforeExpiry);
        Task<ApiResult<IEnumerable<Slider>>> GetExpiringSliders(int daysBeforeExpiry);

    }
}