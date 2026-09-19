using System.ComponentModel.DataAnnotations;

namespace Nexuscomm.API.DTOs
{
    // Used by POST /api/communications/{id}/schedule to (re)schedule an existing draft
    public class ScheduleCommunicationDto
    {
        [Required]
        public DateTime ScheduledAt { get; set; }
    }
}