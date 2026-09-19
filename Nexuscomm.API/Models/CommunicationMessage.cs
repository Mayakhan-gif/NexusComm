using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nexuscomm.API.Enums;

namespace Nexuscomm.API.Models
{
    public class CommunicationMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        public CommunicationChannel Channel { get; set; }

        [Required]
        [MaxLength(256)]
        public string Recipient { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Subject { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required]
        public MessageStatus Status { get; set; } = MessageStatus.Draft;

        public DateTime? ScheduledAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessingStartedAt { get; set; }

        public DateTime? SentAt { get; set; }

        public int RetryCount { get; set; } = 0;

        public int MaxRetryAttempts { get; set; } = 3;

        public DateTime? LastAttemptAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        [MaxLength(1000)]
        public string? FailureReason { get; set; }

        [MaxLength(256)]
        public string? ProviderMessageId { get; set; }

        public string? ProviderResponse { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CommunicationAttempt> Attempts { get; set; } = new List<CommunicationAttempt>();
    }
}