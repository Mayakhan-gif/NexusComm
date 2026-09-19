using Nexuscomm.API.Enums;
using Nexuscomm.API.Models;

namespace Nexuscomm.API.Repositories.Interfaces
{
    public interface ICommunicationRepository
    {
        Task<CommunicationMessage?> GetByIdAsync(int id);

        Task<CommunicationMessage?> GetByIdWithDetailsAsync(int id);

        Task<List<CommunicationMessage>> GetAllForUserAsync(string userId);

        Task<List<CommunicationMessage>> GetAllAsync();

        Task<List<CommunicationMessage>> GetByStatusForUserAsync(string userId, MessageStatus status);

        Task<List<CommunicationMessage>> GetDueForDispatchAsync(DateTime nowUtc, int batchSize);

        // Atomically claims a message for processing. Returns true only if THIS call
        // successfully changed the status (prevents two workers from dispatching the same message).
        Task<bool> TryClaimForDispatchAsync(int messageId, DateTime nowUtc);

        Task AddAsync(CommunicationMessage message);

        void Update(CommunicationMessage message);

        void Remove(CommunicationMessage message);

        Task<int> SaveChangesAsync();
    }
}