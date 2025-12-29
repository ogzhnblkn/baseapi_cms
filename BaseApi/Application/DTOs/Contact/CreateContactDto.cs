using System.ComponentModel.DataAnnotations;

namespace BaseApi.Application.DTOs.Contact
{
    public class CreateContactDto
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter arasında olmalıdır")]
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Ad Soyad sadece harflerden oluşmalıdır")]
        public string NameSurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta 100 karakteri geçemez")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Telefon numarası 20 karakteri geçemez")]
        [RegularExpression(@"^[+]?[0-9\s\-\(\)]+$", ErrorMessage = "Geçersiz telefon numarası formatı")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir zorunludur")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Şehir 2-50 karakter arasında olmalıdır")]
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Şehir sadece harflerden oluşmalıdır")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Konu 100 karakteri geçemez")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Mesaj 10-1000 karakter arasında olmalıdır")]
        public string Message { get; set; } = string.Empty;
    }
}
