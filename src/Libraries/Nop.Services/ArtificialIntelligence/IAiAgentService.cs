using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Stores prototype AI agents in settings. Does not call a provider and does not persist secrets.
/// </summary>
public partial interface IAiAgentService
{
    Task<IList<AiAgent>> GetAllAsync();

    Task<AiAgent> GetByIdAsync(Guid id);

    /// <summary>
    /// Saves wizard fields and leaves the agent disabled until review and a successful connection check.
    /// </summary>
    /// <param name="draft">Fields from the wizard. Enable and review flags on this instance are ignored.</param>
    /// <param name="postedSecret">Value typed into the masked field. Discarded after the configured flag is resolved.</param>
    /// <param name="postedAlreadyConfigured">Previous flag from the form, so a redisplayed mask still counts as configured.</param>
    Task<AiAgent> SaveFromWizardAsync(AiAgent draft, string postedSecret, bool postedAlreadyConfigured);

    Task<bool> SetPermissionsReviewedAsync(Guid id, bool reviewed);

    Task<AiAgent> TestConnectionAsync(Guid id);

    Task<bool> SetEnabledAsync(Guid id, bool enabled);

    Task<bool> DeleteAsync(Guid id);

    Task DeleteAllAsync();

    Task<int> LoadExamplesAsync();
}
