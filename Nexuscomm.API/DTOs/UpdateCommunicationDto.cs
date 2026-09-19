using System.ComponentModel.DataAnnotations;

namespace Nexuscomm.API.DTOs
{
    // Only allowed while message is still Draft/Scheduled (enforced in service layer)
    public class UpdateCommunicationDto
    {
        [MaxLength(256)]
        public string? Recipient { get; set; }

        [MaxLength(256)]
        public string? Subject { get; set; }

        public string? Body { get; set; }

        public DateTime? ScheduledAt { get; set; }
    }
}