namespace Nop.Core.Domain.Common;

/// <summary>
/// Represents a message in the public storefront agent interaction window
/// </summary>
public partial class AgentConversationMessage : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message was sent by the customer
    /// </summary>
    public bool IsFromCustomer { get; set; }

    /// <summary>
    /// Gets or sets the message text
    /// </summary>
    public string MessageText { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
