using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nexuscomm.API.Enums;

namespace Nexuscomm.API.Models
{
    // One record per dispatch try. Never overwritten - always insert new rows.
    public class CommunicationAttempt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommunicationMessageId { get; set; }

        [ForeignKey(nameof(CommunicationMessageId))]
        public CommunicationMessage? CommunicationMessage { get; set; }

        [Required]
        public int AttemptNumber { get; set; }

        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public AttemptStatus Status { get; set; }

        [MaxLength(1000)]
        public string? FailureReason { get; set; }

        public string? ProviderResponse { get; set; }

        [MaxLength(256)]
        public string? ProviderMessageId { get; set; }
    }
}