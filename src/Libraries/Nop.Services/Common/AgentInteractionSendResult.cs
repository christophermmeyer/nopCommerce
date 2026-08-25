using Nop.Core.Domain.Common;

namespace Nop.Services.Common;

/// <summary>
/// Represents the result of sending a storefront agent message
/// </summary>
public partial class AgentInteractionSendResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the message was accepted
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an error resource key when the message was rejected
    /// </summary>
    public string ErrorResourceKey { get; set; }

    /// <summary>
    /// Gets or sets the stored customer message
    /// </summary>
    public AgentConversationMessage CustomerMessage { get; set; }

    /// <summary>
    /// Gets or sets the stored agent reply
    /// </summary>
    public AgentConversationMessage AgentMessage { get; set; }
}
