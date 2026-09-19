using Nexuscomm.API.DTOs;

namespace Nexuscomm.API.Services.Interfaces
{
    public interface ICommunicationService
    {
        Task<CommunicationResponseDto> CreateAsync(string userId, CreateCommunicationDto dto);

        Task<CommunicationResponseDto> GetByIdAsync(int id, string userId, bool isAdmin);

        Task<List<CommunicationListDto>> GetAllForUserAsync(string userId);

        Task<List<CommunicationListDto>> GetAllAsync(); // admin only

        Task<CommunicationResponseDto> UpdateAsync(int id, string userId, bool isAdmin, UpdateCommunicationDto dto);

        Task CancelAsync(int id, string userId, bool isAdmin);

        Task<List<CommunicationAttemptDto>> GetAttemptsAsync(int id, string userId, bool isAdmin);
    }
}