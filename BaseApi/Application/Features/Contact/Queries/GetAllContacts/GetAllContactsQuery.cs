using BaseApi.Application.DTOs.Contact;
using MediatR;

namespace BaseApi.Application.Features.Contact.Queries.GetAllContacts
{
    public class GetAllContactsQuery : IRequest<IEnumerable<ContactDto>>
    {
    }
}
