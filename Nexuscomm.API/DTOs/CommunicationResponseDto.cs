using Nexuscomm.API.Enums;

namespace Nexuscomm.API.DTOs
{
    // Full detail - used for GET /api/communications/{id} (Message Details page)
    public class CommunicationResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public CommunicationChannel Channel { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public MessageStatus Status { get; set; }

        public DateTime? ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessingStartedAt { get; set; }
        public DateTime? SentAt { get; set; }

        public int RetryCount { get; set; }
        public int MaxRetryAttempts { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public DateTime? NextRetryAt { get; set; }

        public string? FailureReason { get; set; }
        public string? ProviderMessageId { get; set; }
        public string? ProviderResponse { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Populated only by GetByIdWithDetailsAsync
        public List<CommunicationAttemptDto> Attempts { get; set; } = new();
    }
}