using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Prototype connection check result.
/// </summary>
public partial class AiAgentConnectionResult
{
    public AiAgentConnectionStatus Status { get; set; }

    public string Message { get; set; }
}
