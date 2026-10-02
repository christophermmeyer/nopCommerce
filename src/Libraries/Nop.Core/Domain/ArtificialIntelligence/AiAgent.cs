namespace Nop.Core.Domain.ArtificialIntelligence;

/// <summary>
/// Admin-configured AI agent stored in settings. Provider secrets are never a property of this type.
/// </summary>
public partial class AiAgent
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public string Description { get; set; }

    public string Provider { get; set; }

    public string Model { get; set; }

    public string EndpointUrl { get; set; }

    /// <summary>
    /// True when an admin typed a placeholder into the masked field. The typed value is discarded.
    /// </summary>
    public bool ApiKeyConfigured { get; set; }

    public List<string> Permissions { get; set; } = new();

    public bool PermissionsReviewed { get; set; }

    public bool Enabled { get; set; }

    public int LastConnectionStatusId { get; set; }

    public string LastConnectionMessage { get; set; }

    public DateTime? LastConnectionTestOnUtc { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }
}
