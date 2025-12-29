using BaseApi.Application.DTOs.Contact;
using BaseApi.Domain.Interfaces;
using MediatR;
namespace BaseApi.Application.Features.Contact.Commands.CreateContact
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactDto>
    {
        private readonly IContactRepository _contactRepository;

        public CreateContactCommandHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {


            var contact = new BaseApi.Domain.Entities.Contact
            {
                City = request.City,
                Email = request.Email,
                Message = request.Message,
                NameSurname = request.NameSurname,
                PhoneNumber = request.PhoneNumber,
                Subject = request.Subject,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _contactRepository.CreateAsync(contact);
            if (!createResult.Success)
                throw new InvalidOperationException(createResult.Message);

            var createdContact = createResult.Data!;

            return new ContactDto
            {
                Id = createdContact.Id,

                NameSurname = createdContact.NameSurname,
                Email = createdContact.Email,
                PhoneNumber = createdContact.PhoneNumber,
                City = createdContact.City,
                Subject = createdContact.Subject,
                Message = createdContact.Message
            };
        }
    }
}
