using System.ComponentModel.DataAnnotations;
using Nexuscomm.API.Enums;

namespace Nexuscomm.API.DTOs
{
    // Used for BOTH "Send Now" and "Schedule" - SendNow flag decides the behavior.
    public class CreateCommunicationDto
    {
        [Required]
        public CommunicationChannel Channel { get; set; }

        [Required]
        [MaxLength(256)]
        public string Recipient { get; set; } = string.Empty;

        // Required only when Channel == Email (validated in the service layer,
        // since DataAnnotations can't easily do conditional-required across properties)
        [MaxLength(256)]
        public string? Subject { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required]
        public bool SendNow { get; set; }

        // Required when SendNow == false
        public DateTime? ScheduledAt { get; set; }
    }
}