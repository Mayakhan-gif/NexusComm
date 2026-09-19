namespace Nexuscomm.API.DTOs
{
    public class DashboardSummaryDto
    {
        public int Total { get; set; }
        public int Scheduled { get; set; }
        public int Processing { get; set; }
        public int Sent { get; set; }
        public int Retrying { get; set; }
        public int Failed { get; set; }
        public int Cancelled { get; set; }

        public List<CommunicationListDto> RecentCommunications { get; set; } = new();
    }
}