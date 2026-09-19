using Nexuscomm.API.Enums;

namespace Nexuscomm.API.DTOs
{
    public class CommunicationAttemptDto
    {
        public int Id { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime AttemptedAt { get; set; }
        public AttemptStatus Status { get; set; }
        public string? FailureReason { get; set; }
        public string? ProviderMessageId { get; set; }
    }
}