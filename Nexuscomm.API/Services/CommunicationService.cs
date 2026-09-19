using Nexuscomm.API.DTOs;
using Nexuscomm.API.Enums;
using Nexuscomm.API.Exceptions;
using Nexuscomm.API.Models;
using Nexuscomm.API.Repositories.Interfaces;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IMessageDispatcher _messageDispatcher;

        public CommunicationService(
            ICommunicationRepository communicationRepository,
            IMessageDispatcher messageDispatcher)
        {
            _communicationRepository = communicationRepository;
            _messageDispatcher = messageDispatcher;
        }

        public async Task<CommunicationResponseDto> CreateAsync(string userId, CreateCommunicationDto dto)
        {
            ValidateCreateRules(dto);

            var message = new CommunicationMessage
            {
                UserId = userId,
                Channel = dto.Channel,
                Recipient = dto.Recipient,
                Subject = dto.Channel == CommunicationChannel.Email ? dto.Subject : null,
                Body = dto.Body,
                Status = dto.SendNow ? MessageStatus.Processing : MessageStatus.Scheduled,
                ScheduledAt = dto.SendNow ? DateTime.UtcNow : dto.ScheduledAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _communicationRepository.AddAsync(message);
            await _communicationRepository.SaveChangesAsync();

            if (dto.SendNow)
            {
                await _messageDispatcher.DispatchAsync(message.Id);

                var dispatched = await _communicationRepository.GetByIdWithDetailsAsync(message.Id);
                return MapToResponseDto(dispatched!);
            }

            return MapToResponseDto(message);
        }

        public async Task<CommunicationResponseDto> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var message = await _communicationRepository.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException($"Communication with ID {id} was not found.");

            EnsureOwnershipOrAdmin(message, userId, isAdmin);

            return MapToResponseDto(message);
        }

        public async Task<List<CommunicationListDto>> GetAllForUserAsync(string userId)
        {
            var messages = await _communicationRepository.GetAllForUserAsync(userId);
            return messages.Select(MapToListDto).ToList();
        }

        public async Task<List<CommunicationListDto>> GetAllAsync()
        {
            var messages = await _communicationRepository.GetAllAsync();
            return messages.Select(MapToListDto).ToList();
        }

        public async Task<CommunicationResponseDto> UpdateAsync(int id, string userId, bool isAdmin, UpdateCommunicationDto dto)
        {
            var message = await _communicationRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Communication with ID {id} was not found.");

            EnsureOwnershipOrAdmin(message, userId, isAdmin);

            if (message.Status != MessageStatus.Draft && message.Status != MessageStatus.Scheduled)
            {
                throw new ValidationException(
                    $"Cannot edit a communication with status '{message.Status}'. Only Draft or Scheduled messages can be edited.");
            }

            if (dto.ScheduledAt.HasValue && dto.ScheduledAt.Value <= DateTime.UtcNow)
            {
                throw new ValidationException("Scheduled time cannot be in the past.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Recipient)) message.Recipient = dto.Recipient;
            if (dto.Subject is not null) message.Subject = dto.Subject;
            if (!string.IsNullOrWhiteSpace(dto.Body)) message.Body = dto.Body;
            if (dto.ScheduledAt.HasValue) message.ScheduledAt = dto.ScheduledAt.Value;

            message.UpdatedAt = DateTime.UtcNow;

            _communicationRepository.Update(message);
            await _communicationRepository.SaveChangesAsync();

            return MapToResponseDto(message);
        }

        public async Task CancelAsync(int id, string userId, bool isAdmin)
        {
            var message = await _communicationRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Communication with ID {id} was not found.");

            EnsureOwnershipOrAdmin(message, userId, isAdmin);

            if (message.Status != MessageStatus.Draft &&
                message.Status != MessageStatus.Scheduled &&
                message.Status != MessageStatus.Retrying)
            {
                throw new ValidationException(
                    $"Cannot cancel a communication with status '{message.Status}'.");
            }

            message.Status = MessageStatus.Cancelled;
            message.UpdatedAt = DateTime.UtcNow;

            _communicationRepository.Update(message);
            await _communicationRepository.SaveChangesAsync();
        }

        public async Task<List<CommunicationAttemptDto>> GetAttemptsAsync(int id, string userId, bool isAdmin)
        {
            var message = await _communicationRepository.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException($"Communication with ID {id} was not found.");

            EnsureOwnershipOrAdmin(message, userId, isAdmin);

            return message.Attempts.Select(MapToAttemptDto).ToList();
        }

        private static void ValidateCreateRules(CreateCommunicationDto dto)
        {
            var errors = new List<string>();

            if (dto.Channel == CommunicationChannel.Email && string.IsNullOrWhiteSpace(dto.Subject))
            {
                errors.Add("Subject is required for Email communications.");
            }

            if (string.IsNullOrWhiteSpace(dto.Recipient))
            {
                errors.Add("Recipient is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Body))
            {
                errors.Add("Message body is required.");
            }

            if (!dto.SendNow)
            {
                if (!dto.ScheduledAt.HasValue)
                {
                    errors.Add("ScheduledAt is required when SendNow is false.");
                }
                else if (dto.ScheduledAt.Value <= DateTime.UtcNow)
                {
                    errors.Add("Scheduled time cannot be in the past.");
                }
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }

        private static void EnsureOwnershipOrAdmin(CommunicationMessage message, string userId, bool isAdmin)
        {
            if (!isAdmin && message.UserId != userId)
            {
                throw new ForbiddenException("You do not have access to this communication.");
            }
        }

        private static CommunicationResponseDto MapToResponseDto(CommunicationMessage m)
        {
            return new CommunicationResponseDto
            {
                Id = m.Id,
                UserId = m.UserId,
                Channel = m.Channel,
                Recipient = m.Recipient,
                Subject = m.Subject,
                Body = m.Body,
                Status = m.Status,
                ScheduledAt = m.ScheduledAt,
                CreatedAt = m.CreatedAt,
                ProcessingStartedAt = m.ProcessingStartedAt,
                SentAt = m.SentAt,
                RetryCount = m.RetryCount,
                MaxRetryAttempts = m.MaxRetryAttempts,
                LastAttemptAt = m.LastAttemptAt,
                NextRetryAt = m.NextRetryAt,
                FailureReason = m.FailureReason,
                ProviderMessageId = m.ProviderMessageId,
                ProviderResponse = m.ProviderResponse,
                UpdatedAt = m.UpdatedAt,
                Attempts = m.Attempts?.Select(MapToAttemptDto).ToList() ?? new List<CommunicationAttemptDto>()
            };
        }

        private static CommunicationListDto MapToListDto(CommunicationMessage m)
        {
            return new CommunicationListDto
            {
                Id = m.Id,
                Channel = m.Channel,
                Recipient = m.Recipient,
                Subject = m.Subject,
                Status = m.Status,
                ScheduledAt = m.ScheduledAt,
                CreatedAt = m.CreatedAt,
                SentAt = m.SentAt,
                RetryCount = m.RetryCount,
                FailureReason = m.FailureReason
            };
        }

        private static CommunicationAttemptDto MapToAttemptDto(CommunicationAttempt a)
        {
            return new CommunicationAttemptDto
            {
                Id = a.Id,
                AttemptNumber = a.AttemptNumber,
                AttemptedAt = a.AttemptedAt,
                Status = a.Status,
                FailureReason = a.FailureReason,
                ProviderMessageId = a.ProviderMessageId
            };
        }
    }
}