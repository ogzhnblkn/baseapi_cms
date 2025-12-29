using BaseApi.Application.DTOs.Contact;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Contact.Queries.GetContact
{
    public class GetContactQueryHandler : IRequestHandler<GetContactQuery, ContactDto?>
    {
        private readonly IContactRepository _ContactRepository;

        public GetContactQueryHandler(IContactRepository ContactRepository)
        {
            _ContactRepository = ContactRepository;
        }

        public async Task<ContactDto?> Handle(GetContactQuery request, CancellationToken cancellationToken)
        {
            var contact = await _ContactRepository.GetByIdAsync(request.Id);
            return contact != null ? new ContactDto
            {
                Id = contact.Data.Id,
                NameSurname = contact.Data.NameSurname,
                Email = contact.Data.Email,
                PhoneNumber = contact.Data.PhoneNumber,
                City = contact.Data.City,
                Subject = contact.Data.Subject,
                Message = contact.Data.Message
            } : null;
        }
    }
}
