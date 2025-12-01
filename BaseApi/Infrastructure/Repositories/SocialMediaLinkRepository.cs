using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class SocialMediaLinkRepository : ISocialMediaLinkRepository
    {
        private readonly ApplicationDbContext _context;

        public SocialMediaLinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SocialMediaLink?> GetByIdAsync(int id)
        {
            return await _context.SocialMediaLinks
                .Include(s => s.Creator)
                .Include(s => s.Updater)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SocialMediaLink>> GetAllAsync()
        {
            return await _context.SocialMediaLinks
                .Include(s => s.Creator)
                .Include(s => s.Updater)
                .OrderBy(s => s.Order)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }



        public async Task<IEnumerable<SocialMediaLink>> GetActiveAsync()
        {
            return await _context.SocialMediaLinks
                .Where(s => s.IsActive)
                .OrderBy(s => s.Order)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<SocialMediaLink> CreateAsync(SocialMediaLink socialMediaLink)
        {
            _context.SocialMediaLinks.Add(socialMediaLink);
            await _context.SaveChangesAsync();
            return socialMediaLink;
        }

        public async Task UpdateAsync(SocialMediaLink socialMediaLink)
        {
            socialMediaLink.UpdatedAt = DateTime.UtcNow;
            _context.SocialMediaLinks.Update(socialMediaLink);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var socialMediaLink = await _context.SocialMediaLinks.FindAsync(id);
            if (socialMediaLink != null)
            {
                _context.SocialMediaLinks.Remove(socialMediaLink);
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementClickCountAsync(int id)
        {
            var socialMediaLink = await _context.SocialMediaLinks.FindAsync(id);
            if (socialMediaLink != null)
            {
                socialMediaLink.ClickCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}