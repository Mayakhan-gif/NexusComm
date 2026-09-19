namespace Nexuscomm.API.Services.Interfaces
{
    // Same result shape as EmailSendResult - keeps the dispatcher's logic symmetric
    public class WhatsAppSendResult
    {
        public bool IsSuccess { get; set; }
        public string? ProviderMessageId { get; set; }
        public string? ProviderResponse { get; set; }
        public string? FailureReason { get; set; }

        public static WhatsAppSendResult Success(string? providerMessageId = null, string? providerResponse = null)
            => new() { IsSuccess = true, ProviderMessageId = providerMessageId, ProviderResponse = providerResponse };

        public static WhatsAppSendResult Failure(string reason, string? providerResponse = null)
            => new() { IsSuccess = false, FailureReason = reason, ProviderResponse = providerResponse };
    }

    public interface IWatiService
    {
        Task<WhatsAppSendResult> SendMessageAsync(string whatsAppNumber, string messageText);
    }
}