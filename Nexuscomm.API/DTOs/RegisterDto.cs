using System.ComponentModel.DataAnnotations;

namespace Nexuscomm.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Optional - no strict format validation here (real-world numbers vary a lot).
        // If you want to enforce a format, do it as a custom check in AuthService instead.
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}