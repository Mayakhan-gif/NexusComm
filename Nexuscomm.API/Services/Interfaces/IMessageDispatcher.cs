namespace Nexuscomm.API.Services.Interfaces
{
    public interface IMessageDispatcher
    {
        // Dispatches a single message by ID: connects to the right provider,
        // records the attempt, and updates the message's status/retry fields.
        Task DispatchAsync(int communicationMessageId);
    }
}