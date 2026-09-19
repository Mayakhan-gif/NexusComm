using Nexuscomm.API.Enums;

namespace Nexuscomm.API.DTOs
{
    // Lightweight version - used for list pages (All/Scheduled/Sent/Failed Messages)
    public class CommunicationListDto
    {
        public int Id { get; set; }
        public CommunicationChannel Channel { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public int RetryCount { get; set; }
        public string? FailureReason { get; set; }
    }
}