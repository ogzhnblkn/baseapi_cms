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

        public async Task<Slider?> GetByIdAsync(int id)
        {
            return await _context.Sliders
                .Include(s => s.Creator)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Slider>> GetAllAsync()
        {
            return await _context.Sliders
                .Include(s => s.Creator)
                .OrderBy(s => s.SliderType)
                .ThenBy(s => s.Order)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Slider>> GetBySliderTypeAsync(SliderType sliderType)
        {
            return await _context.Sliders
                .Where(s => s.SliderType == sliderType)
                .OrderBy(s => s.Order)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Slider>> GetActiveSlidersByTypeAsync(SliderType sliderType)
        {
            var now = DateTime.UtcNow;

            return await _context.Sliders
                .Where(s => s.SliderType == sliderType &&
                           s.IsActive &&
                           (s.StartDate == null || s.StartDate <= now) &&
                           (s.EndDate == null || s.EndDate >= now))
                .OrderBy(s => s.Order)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Slider>> GetSlidersByLocationAsync(string targetLocation)
        {
            return await _context.Sliders
                .Where(s => s.TargetLocation == targetLocation)
                .OrderBy(s => s.Order)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Slider>> GetActiveSlidersByLocationAsync(string targetLocation)
        {
            var now = DateTime.UtcNow;

            return await _context.Sliders
                .Where(s => s.TargetLocation == targetLocation &&
                           s.IsActive &&
                           (s.StartDate == null || s.StartDate <= now) &&
                           (s.EndDate == null || s.EndDate >= now))
                .OrderBy(s => s.Order)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Slider>> GetActiveSlidersInDateRangeAsync(SliderType? sliderType = null)
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

            return await query
                .OrderBy(s => s.SliderType)
                .ThenBy(s => s.Order)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Slider> CreateAsync(Slider slider)
        {
            // Set order if not provided
            if (slider.Order == 0)
            {
                slider.Order = await GetMaxOrderAsync(slider.SliderType) + 1;
            }

            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();
            return slider;
        }

        public async Task<Slider> UpdateAsync(Slider slider)
        {
            slider.UpdatedAt = DateTime.UtcNow;
            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();
            return slider;
        }

        public async Task DeleteAsync(int id)
        {
            var slider = await GetByIdAsync(id);
            if (slider != null)
            {
                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetMaxOrderAsync(SliderType sliderType)
        {
            var sliders = await _context.Sliders
                .Where(s => s.SliderType == sliderType)
                .ToListAsync();

            if (!sliders.Any())
                return 0;

            return sliders.Max(s => s.Order);
        }

        public async Task ReorderSlidersAsync(IEnumerable<(int Id, int Order)> sliderOrders)
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
        }

        public async Task IncrementClickCountAsync(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider != null)
            {
                slider.ClickCount++;
                slider.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider != null)
            {
                slider.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Slider>> GetExpiringSliders(int daysBeforeExpiry)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);

            return await _context.Sliders
                .Where(s => s.EndDate.HasValue &&
                           s.EndDate.Value <= targetDate &&
                           s.EndDate.Value >= DateTime.UtcNow &&
                           s.IsActive)
                .Include(s => s.Creator)
                .ToListAsync();
        }
    }
}