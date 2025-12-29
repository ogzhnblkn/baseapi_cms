using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<Contact>> CreateAsync(Contact Contact)
        {
            _context.Contacts.Add(Contact);
            await _context.SaveChangesAsync();
            return ApiResult<Contact>.SuccessResult(Contact);
        }

        public async Task<ApiResult<IEnumerable<Contact>>> GetAllAsync()
        {
            var Contacts = await _context.Contacts.ToListAsync();
            return ApiResult<IEnumerable<Contact>>.SuccessResult(Contacts);
        }

        public Task<ApiResult<Contact?>> GetByIdAsync(int id)
        {
            var Conbtact = _context.Contacts.FindAsync(id);
            return Task.FromResult(ApiResult<Contact?>.SuccessResult(Conbtact.Result));
        }
    }
}
