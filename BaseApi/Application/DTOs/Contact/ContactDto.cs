namespace BaseApi.Application.DTOs.Contact
{
    public class ContactDto
    {

        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

    }
}
