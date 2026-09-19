namespace Nexuscomm.API.Services.Interfaces
{
    // Result of a single send attempt - the dispatcher uses this to decide
    // Sent vs Failed and what FailureReason to store.
    public class EmailSendResult
    {
        public bool IsSuccess { get; set; }
        public string? ProviderMessageId { get; set; }
        public string? ProviderResponse { get; set; }
        public string? FailureReason { get; set; }

        public static EmailSendResult Success(string? providerMessageId = null, string? providerResponse = null)
            => new() { IsSuccess = true, ProviderMessageId = providerMessageId, ProviderResponse = providerResponse };

        public static EmailSendResult Failure(string reason, string? providerResponse = null)
            => new() { IsSuccess = false, FailureReason = reason, ProviderResponse = providerResponse };
    }

    public interface IEmailService
    {
        Task<EmailSendResult> SendEmailAsync(string toEmail, string subject, string body);
    }
}