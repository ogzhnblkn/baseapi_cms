using BaseApi.Application.DTOs.Contact;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Contact.Queries.GetAllContacts
{
    public class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
    {
        private readonly IContactRepository _ContactRepository;

        public GetAllContactsQueryHandler(IContactRepository ContactRepository)
        {
            _ContactRepository = ContactRepository;
        }
        public async Task<IEnumerable<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var result = await _ContactRepository.GetAllAsync();
            var contacts = result.Data ?? Enumerable.Empty<Domain.Entities.Contact>();

            return contacts.Select(contact => new ContactDto
            {
                City = contact.City,
                NameSurname = contact.NameSurname,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                Subject = contact.Subject,
                Message = contact.Message,
                Id = contact.Id
            });
        }

    }
}
