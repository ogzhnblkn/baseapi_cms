using BaseApi.Application.DTOs.Contact;
using MediatR;

namespace BaseApi.Application.Features.Contact.Queries.GetContact
{
    public class GetContactQuery : IRequest<ContactDto?>
    {
        public int Id { get; set; }
    }

}
