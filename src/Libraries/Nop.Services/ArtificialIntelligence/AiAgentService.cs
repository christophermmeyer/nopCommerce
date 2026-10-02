using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.Configuration;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Persists agents as one setting row each. Setting.Value is limited to 6000 characters, so a single JSON blob is the wrong shape.
/// </summary>
public partial class AiAgentService : IAiAgentService
{
    private const string IdsKey = "aiagentsettings.ids";
    private const int MaxStoredJsonLength = 5800;

    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc
    };

    private readonly ISettingService _settingService;

    public AiAgentService(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public virtual async Task<IList<AiAgent>> GetAllAsync()
    {
        var agents = new List<AiAgent>();
        foreach (var id in await GetIdsAsync())
        {
            var agent = await ReadAsync(id);
            if (agent != null)
                agents.Add(agent);
        }

        return agents;
    }

    public virtual async Task<AiAgent> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await ReadAsync(id);
    }

    public virtual async Task<AiAgent> SaveFromWizardAsync(AiAgent draft, string postedSecret, bool postedAlreadyConfigured)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var existing = draft.Id == Guid.Empty ? null : await GetByIdAsync(draft.Id);
        if (draft.Id != Guid.Empty && existing == null)
            return null;

        var agent = existing ?? new AiAgent
        {
            Id = Guid.NewGuid(),
            CreatedOnUtc = DateTime.UtcNow
        };

        agent.Name = draft.Name?.Trim();
        agent.Role = AiAgentCatalog.Roles.All.Contains(draft.Role) ? draft.Role : AiAgentCatalog.Roles.ProductDescriptions;
        agent.Provider = AiAgentCatalog.Providers.All.Contains(draft.Provider) ? draft.Provider : AiAgentCatalog.Providers.OpenAiCompatible;
        agent.Description = TrimTo(draft.Description, 500);
        agent.Model = TrimTo(draft.Model, 120);
        agent.EndpointUrl = TrimTo(draft.EndpointUrl, 500);
        agent.Permissions = AiAgentPrototypePolicy.NormalizePermissions(draft.Permissions);

        var alreadyConfigured = (existing?.ApiKeyConfigured ?? false) || postedAlreadyConfigured;
        AiAgentPrototypePolicy.ScrubPostedSecret(agent, postedSecret);
        agent.ApiKeyConfigured = AiAgentPrototypePolicy.ResolveApiKeyConfigured(postedSecret, alreadyConfigured);

        AiAgentPrototypePolicy.MarkNeedsReview(agent);
        agent.UpdatedOnUtc = DateTime.UtcNow;

        await WriteAsync(agent);
        return agent;
    }

    public virtual async Task<bool> SetPermissionsReviewedAsync(Guid id, bool reviewed)
    {
        var agent = await GetByIdAsync(id);
        if (agent == null)
            return false;

        agent.PermissionsReviewed = reviewed;
        if (!reviewed)
            agent.Enabled = false;

        agent.UpdatedOnUtc = DateTime.UtcNow;
        await WriteAsync(agent);
        return true;
    }

    public virtual async Task<AiAgent> TestConnectionAsync(Guid id)
    {
        var agent = await GetByIdAsync(id);
        if (agent == null)
            return null;

        var result = AiAgentPrototypePolicy.Test(agent);
        AiAgentPrototypePolicy.ApplyTestResult(agent, result, DateTime.UtcNow);
        agent.UpdatedOnUtc = DateTime.UtcNow;
        await WriteAsync(agent);
        return agent;
    }

    public virtual async Task<bool> SetEnabledAsync(Guid id, bool enabled)
    {
        var agent = await GetByIdAsync(id);
        if (agent == null)
            return false;

        if (enabled && !AiAgentPrototypePolicy.CanEnable(agent))
            return false;

        agent.Enabled = enabled;
        agent.UpdatedOnUtc = DateTime.UtcNow;
        await WriteAsync(agent);
        return true;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var ids = await GetIdsAsync();
        if (!ids.Contains(id))
            return false;

        await DeleteSettingAsync(EntryKey(id));
        ids.Remove(id);
        await SaveIdsAsync(ids);
        return true;
    }

    public virtual async Task DeleteAllAsync()
    {
        foreach (var id in await GetIdsAsync())
            await DeleteSettingAsync(EntryKey(id));

        await SaveIdsAsync(new List<Guid>());
    }

    public virtual async Task<int> LoadExamplesAsync()
    {
        var added = 0;
        foreach (var example in BuildExamples())
        {
            if (await GetByIdAsync(example.Id) != null)
                continue;

            await WriteAsync(example);
            added++;
        }

        return added;
    }

    private async Task<AiAgent> ReadAsync(Guid id)
    {
        var json = await _settingService.GetSettingByKeyAsync<string>(EntryKey(id), string.Empty);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonConvert.DeserializeObject<AiAgent>(json, _jsonSettings);
    }

    private async Task WriteAsync(AiAgent agent)
    {
        var json = JsonConvert.SerializeObject(agent, _jsonSettings);
        if (json.Length > MaxStoredJsonLength)
            throw new NopException("This agent is too large to store in settings. Shorten the description and try again.");

        await _settingService.SetSettingAsync(EntryKey(agent.Id), json);

        var ids = await GetIdsAsync();
        if (!ids.Contains(agent.Id))
        {
            ids.Add(agent.Id);
            await SaveIdsAsync(ids);
        }
    }

    private async Task<List<Guid>> GetIdsAsync()
    {
        var raw = await _settingService.GetSettingByKeyAsync<string>(IdsKey, string.Empty);
        if (string.IsNullOrWhiteSpace(raw))
            return new List<Guid>();

        var ids = new List<Guid>();
        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Guid.TryParse(part, out var id))
                ids.Add(id);
        }

        return ids;
    }

    private async Task SaveIdsAsync(IList<Guid> ids)
    {
        var value = string.Join(",", ids.Select(id => id.ToString("N")));
        await _settingService.SetSettingAsync(IdsKey, value);
    }

    private async Task DeleteSettingAsync(string key)
    {
        var setting = await _settingService.GetSettingAsync(key);
        if (setting != null)
            await _settingService.DeleteSettingAsync(setting);
    }

    private static string EntryKey(Guid id)
    {
        return $"aiagentsettings.agent.{id:N}";
    }

    private static string TrimTo(string value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();
        return value.Length <= max ? value : value[..max];
    }

    private static IEnumerable<AiAgent> BuildExamples()
    {
        yield return new AiAgent
        {
            Id = Guid.Parse("4f3c2a10-6b1e-4d4a-9c11-a1a1a1a1a101"),
            Name = "Catalog copy assistant",
            Role = AiAgentCatalog.Roles.ProductDescriptions,
            Description = "Drafts clear product descriptions from attributes and short notes. Does not publish without admin review.",
            Provider = AiAgentCatalog.Providers.OpenAiCompatible,
            Model = "gpt-4.1-mini",
            EndpointUrl = "https://catalog-assistant.test/v1",
            ApiKeyConfigured = true,
            Permissions = new List<string>
            {
                AiAgentCatalog.Permissions.ReadProductCatalog,
                AiAgentCatalog.Permissions.DraftProductDescriptions,
                AiAgentCatalog.Permissions.SuggestSeoMeta
            },
            PermissionsReviewed = true,
            Enabled = true,
            LastConnectionStatusId = (int)AiAgentConnectionStatus.Success,
            LastConnectionMessage = "Prototype check passed. Settings look complete. No live provider was contacted, and no API key was sent.",
            CreatedOnUtc = new DateTime(2026, 9, 18, 0, 0, 0, DateTimeKind.Utc),
            UpdatedOnUtc = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc),
            LastConnectionTestOnUtc = new DateTime(2026, 9, 24, 0, 0, 0, DateTimeKind.Utc)
        };

        yield return new AiAgent
        {
            Id = Guid.Parse("4f3c2a10-6b1e-4d4a-9c11-a1a1a1a1a102"),
            Name = "Support reply drafter",
            Role = AiAgentCatalog.Roles.CustomerServiceDrafts,
            Description = "Drafts support replies. Still waiting on a permissions review.",
            Provider = AiAgentCatalog.Providers.Anthropic,
            Model = "claude-sonnet",
            EndpointUrl = "https://api.example.com/v1",
            ApiKeyConfigured = true,
            Permissions = new List<string>
            {
                AiAgentCatalog.Permissions.ReadCustomerMessages,
                AiAgentCatalog.Permissions.DraftSupportReplies
            },
            PermissionsReviewed = false,
            Enabled = false,
            LastConnectionStatusId = (int)AiAgentConnectionStatus.Misconfigured,
            LastConnectionMessage = "Endpoint host 'api.example.com' is still a placeholder. Replace it before enable. This is a configuration problem, not a stopped agent.",
            CreatedOnUtc = new DateTime(2026, 9, 20, 0, 0, 0, DateTimeKind.Utc),
            UpdatedOnUtc = new DateTime(2026, 9, 22, 0, 0, 0, DateTimeKind.Utc),
            LastConnectionTestOnUtc = new DateTime(2026, 9, 22, 0, 0, 0, DateTimeKind.Utc)
        };

        yield return new AiAgent
        {
            Id = Guid.Parse("4f3c2a10-6b1e-4d4a-9c11-a1a1a1a1a103"),
            Name = "SEO meta helper",
            Role = AiAgentCatalog.Roles.MetaTitlesAndDescriptions,
            Description = "Suggests meta titles and descriptions. Disabled after an unreachable check.",
            Provider = AiAgentCatalog.Providers.AzureOpenAi,
            Model = "gpt-4.1-mini",
            EndpointUrl = "https://unreachable.invalid/v1",
            ApiKeyConfigured = true,
            Permissions = new List<string>
            {
                AiAgentCatalog.Permissions.ReadProductCatalog,
                AiAgentCatalog.Permissions.SuggestSeoMeta
            },
            PermissionsReviewed = true,
            Enabled = false,
            LastConnectionStatusId = (int)AiAgentConnectionStatus.Unreachable,
            LastConnectionMessage = "Settings look complete, but the endpoint did not respond. This is a dead agent, not a missing setting. No live provider was contacted.",
            CreatedOnUtc = new DateTime(2026, 9, 16, 0, 0, 0, DateTimeKind.Utc),
            UpdatedOnUtc = new DateTime(2026, 9, 18, 0, 0, 0, DateTimeKind.Utc),
            LastConnectionTestOnUtc = new DateTime(2026, 9, 18, 0, 0, 0, DateTimeKind.Utc)
        };
    }
}
