using Microsoft.EntityFrameworkCore;
using Nexuscomm.API.Data;
using Nexuscomm.API.Enums;
using Nexuscomm.API.Models;
using Nexuscomm.API.Repositories.Interfaces;

namespace Nexuscomm.API.Repositories
{
    public class CommunicationRepository : ICommunicationRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommunicationMessage?> GetByIdAsync(int id)
        {
            return await _context.CommunicationMessages
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<CommunicationMessage?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.CommunicationMessages
                .Include(m => m.Attempts.OrderBy(a => a.AttemptNumber))
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<CommunicationMessage>> GetAllForUserAsync(string userId)
        {
            return await _context.CommunicationMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CommunicationMessage>> GetAllAsync()
        {
            return await _context.CommunicationMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CommunicationMessage>> GetByStatusForUserAsync(string userId, MessageStatus status)
        {
            return await _context.CommunicationMessages
                .Where(m => m.UserId == userId && m.Status == status)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CommunicationMessage>> GetDueForDispatchAsync(DateTime nowUtc, int batchSize)
        {
            // AsNoTracking: these entities are only used to read IDs (worker loops over them
            // and calls TryClaimForDispatchAsync + DispatchAsync separately). Without this,
            // EF Core keeps a stale cached copy in memory, and the later fetch inside
            // DispatchAsync incorrectly returns the OLD status instead of the freshly
            // updated "Processing" status.
            return await _context.CommunicationMessages
                .AsNoTracking()
                .Where(m =>
                    (m.Status == MessageStatus.Scheduled && m.ScheduledAt != null && m.ScheduledAt <= nowUtc) ||
                    (m.Status == MessageStatus.Retrying && m.NextRetryAt != null && m.NextRetryAt <= nowUtc))
                .OrderBy(m => m.ScheduledAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<bool> TryClaimForDispatchAsync(int messageId, DateTime nowUtc)
        {
            // ExecuteUpdateAsync runs a single atomic UPDATE ... WHERE statement directly in SQL.
            // If another worker already claimed this row (changed its Status), the WHERE clause
            // here will match 0 rows, and rowsAffected will be 0 - meaning we back off safely.
            var rowsAffected = await _context.CommunicationMessages
                .Where(m => m.Id == messageId &&
                            (m.Status == MessageStatus.Scheduled || m.Status == MessageStatus.Retrying))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Status, MessageStatus.Processing)
                    .SetProperty(m => m.ProcessingStartedAt, nowUtc)
                    .SetProperty(m => m.UpdatedAt, nowUtc));

            return rowsAffected > 0;
        }

        public async Task AddAsync(CommunicationMessage message)
        {
            await _context.CommunicationMessages.AddAsync(message);
        }

        public void Update(CommunicationMessage message)
        {
            _context.CommunicationMessages.Update(message);
        }

        public void Remove(CommunicationMessage message)
        {
            _context.CommunicationMessages.Remove(message);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}