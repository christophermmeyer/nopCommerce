namespace Nop.Services.Common;

/// <summary>
/// Represents a classified intent for a storefront agent message
/// </summary>
public enum AgentIntent
{
    /// <summary>
    /// Unrecognized request
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Greeting
    /// </summary>
    Greeting = 1,

    /// <summary>
    /// Order status or tracking
    /// </summary>
    OrderStatus = 2,

    /// <summary>
    /// Shipping or delivery
    /// </summary>
    Shipping = 3,

    /// <summary>
    /// Returns or refunds
    /// </summary>
    Returns = 4,

    /// <summary>
    /// Payment or checkout
    /// </summary>
    Payment = 5,

    /// <summary>
    /// Account, login, or password
    /// </summary>
    Account = 6,

    /// <summary>
    /// Product search or recommendations
    /// </summary>
    ProductSearch = 7,

    /// <summary>
    /// Contact or human support
    /// </summary>
    Contact = 8
}
