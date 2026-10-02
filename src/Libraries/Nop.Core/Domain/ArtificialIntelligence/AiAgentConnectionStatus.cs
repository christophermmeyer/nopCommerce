namespace Nop.Core.Domain.ArtificialIntelligence;

/// <summary>
/// Result of the prototype connection check. No provider is contacted.
/// </summary>
public enum AiAgentConnectionStatus
{
    /// <summary>
    /// Check has not been run since the last edit.
    /// </summary>
    NotTested = 0,

    /// <summary>
    /// Settings look complete. Still not a live provider call.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Settings are incomplete or still use a placeholder. Not a dead agent.
    /// </summary>
    Misconfigured = 2,

    /// <summary>
    /// Settings look complete, but the endpoint is treated as unreachable.
    /// </summary>
    Unreachable = 3
}
