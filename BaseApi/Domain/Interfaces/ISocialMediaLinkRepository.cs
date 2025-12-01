using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface ISocialMediaLinkRepository
    {
        Task<SocialMediaLink?> GetByIdAsync(int id);
        Task<IEnumerable<SocialMediaLink>> GetAllAsync();
        Task<IEnumerable<SocialMediaLink>> GetActiveAsync();
        Task<SocialMediaLink> CreateAsync(SocialMediaLink socialMediaLink);
        Task UpdateAsync(SocialMediaLink socialMediaLink);
        Task DeleteAsync(int id);
        Task IncrementClickCountAsync(int id);
    }
}