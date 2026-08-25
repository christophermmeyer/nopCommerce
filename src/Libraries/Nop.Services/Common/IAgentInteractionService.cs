using Nop.Core.Domain.Common;

namespace Nop.Services.Common;

/// <summary>
/// Agent interaction service
/// </summary>
public partial interface IAgentInteractionService
{
    /// <summary>
    /// Gets recent conversation messages for a customer in a store
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the conversation messages
    /// </returns>
    Task<IList<AgentConversationMessage>> GetConversationMessagesAsync(int customerId, int storeId);

    /// <summary>
    /// Sends a customer message and returns the stored customer message plus the agent reply
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="message">Customer message text</param>
    /// <param name="storeName">Current store name used in replies</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the send result
    /// </returns>
    Task<AgentInteractionSendResult> SendMessageAsync(int customerId, int storeId, string message, string storeName);
}
