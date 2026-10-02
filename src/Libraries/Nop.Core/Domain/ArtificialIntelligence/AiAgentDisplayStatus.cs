namespace Nop.Core.Domain.ArtificialIntelligence;

/// <summary>
/// Admin list status. Connection health is reported separately.
/// </summary>
public enum AiAgentDisplayStatus
{
    /// <summary>
    /// Agent is enabled after review and a successful check.
    /// </summary>
    Enabled = 0,

    /// <summary>
    /// Saved, but permissions have not been confirmed.
    /// </summary>
    NeedsReview = 1,

    /// <summary>
    /// Permissions were reviewed and the agent is turned off.
    /// </summary>
    Disabled = 2
}
