using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class SliderRepository : ISliderRepository
    {
        private readonly ApplicationDbContext _context;

        public SliderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<Slider?>> GetByIdAsync(int id)
        {
            try
            {
                var slider = await _context.Sliders
                    .Include(s => s.Creator)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (slider == null)
                    return ApiResult<Slider?>.NotFoundResult(Messages.Slider.NotFound);

                return ApiResult<Slider?>.SuccessResult(slider, Messages.Slider.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<Slider?>.FailureResult($"Error retrieving slider: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetAllAsync()
        {
            try
            {
                var sliders = await _context.Sliders
                    .Include(s => s.Creator)
                    .OrderBy(s => s.SliderType)
                    .ThenBy(s => s.Order)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, Messages.Slider.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving sliders: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetBySliderTypeAsync(SliderType sliderType)
        {
            try
            {
                var sliders = await _context.Sliders
                    .Where(s => s.SliderType == sliderType)
                    .OrderBy(s => s.Order)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, Messages.Slider.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving sliders by type: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetActiveSlidersByTypeAsync(SliderType sliderType)
        {
            try
            {
                var now = DateTime.UtcNow;

                var sliders = await _context.Sliders
                    .Where(s => s.SliderType == sliderType &&
                               s.IsActive &&
                               (s.StartDate == null || s.StartDate <= now) &&
                               (s.EndDate == null || s.EndDate >= now))
                    .OrderBy(s => s.Order)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, Messages.Slider.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving active sliders: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetSlidersByLocationAsync(string targetLocation)
        {
            try
            {
                var sliders = await _context.Sliders
                    .Where(s => s.TargetLocation == targetLocation)
                    .OrderBy(s => s.Order)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, Messages.Slider.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving sliders by location: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetActiveSlidersByLocationAsync(string targetLocation)
        {
            try
            {
                var now = DateTime.UtcNow;

                var sliders = await _context.Sliders
                    .Where(s => s.TargetLocation == targetLocation &&
                               s.IsActive &&
                               (s.StartDate == null || s.StartDate <= now) &&
                               (s.EndDate == null || s.EndDate >= now))
                    .OrderBy(s => s.Order)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, Messages.Slider.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving active sliders by location: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetActiveSlidersInDateRangeAsync(SliderType? sliderType = null)
        {
            try
            {
                var now = DateTime.UtcNow;
                var query = _context.Sliders
                    .Where(s => s.IsActive &&
                               (s.StartDate == null || s.StartDate <= now) &&
                               (s.EndDate == null || s.EndDate >= now));

                if (sliderType.HasValue)
                {
                    query = query.Where(s => s.SliderType == sliderType.Value);
                }

                var sliders = await query
                    .OrderBy(s => s.SliderType)
                    .ThenBy(s => s.Order)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, Messages.Slider.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving active sliders in date range: {ex.Message}");
            }
        }

        public async Task<ApiResult<Slider>> CreateAsync(Slider slider)
        {
            try
            {
                // Set order if not provided
                if (slider.Order == 0)
                {
                    var maxOrderResult = await GetMaxOrderAsync(slider.SliderType);
                    slider.Order = (maxOrderResult?.Data ?? 0) + 1;
                }

                _context.Sliders.Add(slider);
                await _context.SaveChangesAsync();

                return ApiResult<Slider>.SuccessResult(slider, Messages.Slider.Created);
            }
            catch (Exception ex)
            {
                return ApiResult<Slider>.FailureResult($"Error creating slider: {ex.Message}");
            }
        }

        public async Task<ApiResult<Slider>> UpdateAsync(Slider slider)
        {
            try
            {
                slider.UpdatedAt = DateTime.UtcNow;
                _context.Sliders.Update(slider);
                await _context.SaveChangesAsync();

                return ApiResult<Slider>.SuccessResult(slider, Messages.Slider.Updated);
            }
            catch (Exception ex)
            {
                return ApiResult<Slider>.FailureResult($"Error updating slider: {ex.Message}");
            }
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            try
            {
                var slider = await _context.Sliders.FindAsync(id);
                if (slider == null)
                    return ApiResult.FailureResult(Messages.Slider.NotFound);

                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult(Messages.Slider.Deleted);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error deleting slider: {ex.Message}");
            }
        }

        public async Task<ApiResult<int>> GetMaxOrderAsync(SliderType sliderType)
        {
            try
            {
                var sliders = await _context.Sliders
                    .Where(s => s.SliderType == sliderType)
                    .ToListAsync();

                if (!sliders.Any())
                    return ApiResult<int>.SuccessResult(0);

                var maxOrder = sliders.Max(s => s.Order);
                return ApiResult<int>.SuccessResult(maxOrder);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.FailureResult($"Error getting max order: {ex.Message}");
            }
        }

        public async Task<ApiResult> ReorderSlidersAsync(IEnumerable<(int Id, int Order)> sliderOrders)
        {
            try
            {
                foreach (var (id, order) in sliderOrders)
                {
                    var slider = await _context.Sliders.FindAsync(id);
                    if (slider != null)
                    {
                        slider.Order = order;
                        slider.UpdatedAt = DateTime.UtcNow;
                    }
                }
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult("Sliders reordered successfully");
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error reordering sliders: {ex.Message}");
            }
        }

        public async Task<ApiResult> IncrementClickCountAsync(int id)
        {
            try
            {
                var slider = await _context.Sliders.FindAsync(id);
                if (slider == null)
                    return ApiResult.FailureResult(Messages.Slider.NotFound);

                slider.ClickCount++;
                slider.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult("Click count incremented successfully");
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error incrementing click count: {ex.Message}");
            }
        }

        public async Task<ApiResult> IncrementViewCountAsync(int id)
        {
            try
            {
                var slider = await _context.Sliders.FindAsync(id);
                if (slider == null)
                    return ApiResult.FailureResult(Messages.Slider.NotFound);

                slider.ViewCount++;
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult("View count incremented successfully");
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error incrementing view count: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Slider>>> GetExpiringSliders(int daysBeforeExpiry)
        {
            try
            {
                var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);

                var sliders = await _context.Sliders
                    .Where(s => s.EndDate.HasValue &&
                               s.EndDate.Value <= targetDate &&
                               s.EndDate.Value >= DateTime.UtcNow &&
                               s.IsActive)
                    .Include(s => s.Creator)
                    .ToListAsync();

                return ApiResult<IEnumerable<Slider>>.SuccessResult(sliders, "Expiring sliders retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Slider>>.FailureResult($"Error retrieving expiring sliders: {ex.Message}");
            }

        }
    }
}