using Microsoft.EntityFrameworkCore;
using Nexuscomm.API.Data;
using Nexuscomm.API.Models;
using Nexuscomm.API.Repositories.Interfaces;

namespace Nexuscomm.API.Repositories
{
    public class CommunicationAttemptRepository : ICommunicationAttemptRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunicationAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommunicationAttempt>> GetByMessageIdAsync(int communicationMessageId)
        {
            return await _context.CommunicationAttempts
                .Where(a => a.CommunicationMessageId == communicationMessageId)
                .OrderBy(a => a.AttemptNumber)
                .ToListAsync();
        }

        public async Task AddAsync(CommunicationAttempt attempt)
        {
            await _context.CommunicationAttempts.AddAsync(attempt);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}