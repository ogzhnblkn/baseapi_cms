using System.ComponentModel.DataAnnotations;

namespace BaseApi.Domain.Entities
{
    public class TokenBlacklist
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public string? Reason { get; set; }

        // Navigation Property
        public User? User { get; set; }
    }
}