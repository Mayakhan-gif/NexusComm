namespace Nexuscomm.API.Configurations
{
    public class RetrySettings
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public int InitialRetryDelayMinutes { get; set; } = 2;
        public int BackoffMultiplier { get; set; } = 2;
    }
}