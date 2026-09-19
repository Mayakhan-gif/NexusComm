using Microsoft.AspNetCore.Identity;

namespace Nexuscomm.API.Models
{
    // Extends Identity's built-in user with extra fields we need.
    // IdentityUser already gives us: Id, Email, UserName, PasswordHash, PhoneNumber, etc.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property: one user can have many communications
        public ICollection<CommunicationMessage> CommunicationMessages { get; set; } = new List<CommunicationMessage>();
    }
}