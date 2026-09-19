using Microsoft.Extensions.Options;
using Nexuscomm.API.Configurations;
using Nexuscomm.API.Enums;
using Nexuscomm.API.Models;
using Nexuscomm.API.Repositories.Interfaces;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Services
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly ICommunicationAttemptRepository _attemptRepository;
        private readonly IEmailService _emailService;
        private readonly IWatiService _watiService;
        private readonly RetrySettings _retrySettings;

        public MessageDispatcher(
            ICommunicationRepository communicationRepository,
            ICommunicationAttemptRepository attemptRepository,
            IEmailService emailService,
            IWatiService watiService,
            IOptions<RetrySettings> retrySettings)
        {
            _communicationRepository = communicationRepository;
            _attemptRepository = attemptRepository;
            _emailService = emailService;
            _watiService = watiService;
            _retrySettings = retrySettings.Value;
        }

        public async Task DispatchAsync(int communicationMessageId)
        {
            var message = await _communicationRepository.GetByIdAsync(communicationMessageId);

            if (message is null)
            {
                return;
            }

            if (message.Status != MessageStatus.Processing)
            {
                return;
            }

            var attemptNumber = message.RetryCount + 1;
            bool isSuccess;
            string? failureReason = null;
            string? providerMessageId = null;
            string? providerResponse = null;

            try
            {
                if (message.Channel == CommunicationChannel.Email)
                {
                    var result = await _emailService.SendEmailAsync(
                        message.Recipient,
                        message.Subject ?? string.Empty,
                        message.Body);

                    isSuccess = result.IsSuccess;
                    failureReason = result.FailureReason;
                    providerMessageId = result.ProviderMessageId;
                    providerResponse = result.ProviderResponse;
                }
                else
                {
                    var result = await _watiService.SendMessageAsync(message.Recipient, message.Body);

                    isSuccess = result.IsSuccess;
                    failureReason = result.FailureReason;
                    providerMessageId = result.ProviderMessageId;
                    providerResponse = result.ProviderResponse;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
                failureReason = "Unexpected error while contacting the provider.";
            }

            var attempt = new CommunicationAttempt
            {
                CommunicationMessageId = message.Id,
                AttemptNumber = attemptNumber,
                AttemptedAt = DateTime.UtcNow,
                Status = isSuccess ? AttemptStatus.Success : AttemptStatus.Failed,
                FailureReason = failureReason,
                ProviderResponse = providerResponse,
                ProviderMessageId = providerMessageId
            };
            await _attemptRepository.AddAsync(attempt);

            message.LastAttemptAt = DateTime.UtcNow;
            message.UpdatedAt = DateTime.UtcNow;
            message.ProviderMessageId = providerMessageId ?? message.ProviderMessageId;
            message.ProviderResponse = providerResponse ?? message.ProviderResponse;

            if (isSuccess)
            {
                message.Status = MessageStatus.Sent;
                message.SentAt = DateTime.UtcNow;
                message.FailureReason = null;
            }
            else
            {
                message.FailureReason = failureReason;
                message.RetryCount = attemptNumber;

                if (attemptNumber >= message.MaxRetryAttempts)
                {
                    message.Status = MessageStatus.Failed;
                    message.NextRetryAt = null;
                }
                else
                {
                    message.Status = MessageStatus.Retrying;

                    var delayMinutes = _retrySettings.InitialRetryDelayMinutes *
                        Math.Pow(_retrySettings.BackoffMultiplier, attemptNumber - 1);

                    message.NextRetryAt = DateTime.UtcNow.AddMinutes(delayMinutes);
                }
            }

            _communicationRepository.Update(message);
            await _communicationRepository.SaveChangesAsync();
        }
    }
}