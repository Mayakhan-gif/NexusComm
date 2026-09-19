using Nexuscomm.API.Repositories.Interfaces;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.BackgroundServices
{
    public class CommunicationDispatchJob
    {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IMessageDispatcher _messageDispatcher;

        private const int BatchSize = 20;

        public CommunicationDispatchJob(
            ICommunicationRepository communicationRepository,
            IMessageDispatcher messageDispatcher)
        {
            _communicationRepository = communicationRepository;
            _messageDispatcher = messageDispatcher;
        }

        public async Task ProcessDueMessagesAsync()
        {
            var nowUtc = DateTime.UtcNow;
            var dueMessages = await _communicationRepository.GetDueForDispatchAsync(nowUtc, BatchSize);

            if (dueMessages.Count == 0)
            {
                return;
            }

            foreach (var message in dueMessages)
            {
                var claimed = await _communicationRepository.TryClaimForDispatchAsync(message.Id, nowUtc);

                if (!claimed)
                {
                    continue;
                }

                try
                {
                    await _messageDispatcher.DispatchAsync(message.Id);
                }
                catch (Exception)
                {
                    // Swallow: a single bad message should not stop the batch.
                }
            }
        }
    }
}