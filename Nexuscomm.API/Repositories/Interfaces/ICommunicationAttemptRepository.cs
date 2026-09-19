using Nexuscomm.API.Models;

namespace Nexuscomm.API.Repositories.Interfaces
{
    public interface ICommunicationAttemptRepository
    {
        Task<List<CommunicationAttempt>> GetByMessageIdAsync(int communicationMessageId);

        Task AddAsync(CommunicationAttempt attempt);

        Task<int> SaveChangesAsync();
    }
}