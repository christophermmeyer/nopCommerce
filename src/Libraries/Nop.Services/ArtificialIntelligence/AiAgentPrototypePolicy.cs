using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Rules for the admin AI-agent prototype. Checks are local: nothing is sent to a provider.
/// </summary>
public static partial class AiAgentPrototypePolicy
{
    /// <summary>
    /// Placeholder hosts (RFC 2606 example.*) and loopback are incomplete configuration.
    /// </summary>
    public static bool IsPlaceholderHost(string host)
    {
        host = NormalizeHost(host);
        if (string.IsNullOrEmpty(host))
            return true;

        if (host is "localhost" or "127.0.0.1" or "::1")
            return true;

        return host is "example.com" or "example.org" or "example.net"
            || host.EndsWith(".example.com", StringComparison.Ordinal)
            || host.EndsWith(".example.org", StringComparison.Ordinal)
            || host.EndsWith(".example.net", StringComparison.Ordinal);
    }

    /// <summary>
    /// *.invalid is reserved and never routed. The prototype uses it as a dead-agent signal.
    /// </summary>
    public static bool IsDeadAgentHost(string host)
    {
        host = NormalizeHost(host);
        return host.EndsWith(".invalid", StringComparison.Ordinal);
    }

    public static AiAgentConnectionResult Test(AiAgent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);

        if (string.IsNullOrWhiteSpace(agent.Model))
            return Misconfigured("Model is missing. This is a configuration problem, not a stopped agent.");

        if (!agent.ApiKeyConfigured)
            return Misconfigured("API key has not been supplied. This prototype stores only a configured flag, never the key. This is a configuration problem, not a stopped agent.");

        if (!Uri.TryCreate((agent.EndpointUrl ?? string.Empty).Trim(), UriKind.Absolute, out var endpoint)
            || (endpoint.Scheme != Uri.UriSchemeHttps && endpoint.Scheme != Uri.UriSchemeHttp)
            || string.IsNullOrEmpty(endpoint.Host))
            return Misconfigured("Endpoint URL must be an absolute http or https URL. This is a configuration problem, not a stopped agent.");

        if (IsPlaceholderHost(endpoint.Host))
            return Misconfigured($"Endpoint host '{endpoint.Host}' is still a placeholder. Replace it before enable. This is a configuration problem, not a stopped agent.");

        if (IsDeadAgentHost(endpoint.Host) || string.Equals(agent.Model.Trim(), "unreachable", StringComparison.OrdinalIgnoreCase))
            return Unreachable("Settings look complete, but the endpoint did not respond. This is a dead agent, not a missing setting. No live provider was contacted.");

        return new AiAgentConnectionResult
        {
            Status = AiAgentConnectionStatus.Success,
            Message = "Prototype check passed. Settings look complete. No live provider was contacted, and no API key was sent."
        };
    }

    public static bool CanEnable(AiAgent agent)
    {
        return agent != null
            && agent.PermissionsReviewed
            && agent.LastConnectionStatusId == (int)AiAgentConnectionStatus.Success;
    }

    public static AiAgentDisplayStatus GetDisplayStatus(AiAgent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);

        if (agent.Enabled)
            return AiAgentDisplayStatus.Enabled;

        if (!agent.PermissionsReviewed)
            return AiAgentDisplayStatus.NeedsReview;

        return AiAgentDisplayStatus.Disabled;
    }

    public static void MarkNeedsReview(AiAgent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);

        agent.Enabled = false;
        agent.PermissionsReviewed = false;
        agent.LastConnectionStatusId = (int)AiAgentConnectionStatus.NotTested;
        agent.LastConnectionMessage = null;
        agent.LastConnectionTestOnUtc = null;
    }

    public static void ApplyTestResult(AiAgent agent, AiAgentConnectionResult result, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentNullException.ThrowIfNull(result);

        agent.LastConnectionStatusId = (int)result.Status;
        agent.LastConnectionMessage = result.Message;
        agent.LastConnectionTestOnUtc = utcNow;

        if (result.Status != AiAgentConnectionStatus.Success)
            agent.Enabled = false;
    }

    /// <summary>
    /// Empty clears the flag. A mask keeps the previous flag. Any other text marks the key configured and is not stored.
    /// </summary>
    public static bool ResolveApiKeyConfigured(string postedSecret, bool alreadyConfigured)
    {
        if (string.IsNullOrWhiteSpace(postedSecret))
            return false;

        if (IsMask(postedSecret))
            return alreadyConfigured;

        return true;
    }

    public static bool IsMask(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        foreach (var character in value.Trim())
        {
            if (character is not ('•' or '*' or '·' or '.'))
                return false;
        }

        return true;
    }

    public static List<string> NormalizePermissions(IEnumerable<string> selected)
    {
        if (selected == null)
            return new List<string>();

        return selected
            .Where(permission => AiAgentCatalog.Permissions.Grantable.Contains(permission))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Drops a long pasted secret from text fields. The secret itself is never assigned onto the agent.
    /// </summary>
    public static void ScrubPostedSecret(AiAgent agent, string postedSecret)
    {
        ArgumentNullException.ThrowIfNull(agent);

        if (string.IsNullOrEmpty(postedSecret) || postedSecret.Length < 12 || IsMask(postedSecret))
            return;

        agent.Name = Remove(agent.Name, postedSecret);
        agent.Description = Remove(agent.Description, postedSecret);
        agent.Model = Remove(agent.Model, postedSecret);
        agent.EndpointUrl = Remove(agent.EndpointUrl, postedSecret);
        agent.Role = Remove(agent.Role, postedSecret);
        agent.Provider = Remove(agent.Provider, postedSecret);
    }

    private static AiAgentConnectionResult Misconfigured(string message)
    {
        return new AiAgentConnectionResult
        {
            Status = AiAgentConnectionStatus.Misconfigured,
            Message = message
        };
    }

    private static AiAgentConnectionResult Unreachable(string message)
    {
        return new AiAgentConnectionResult
        {
            Status = AiAgentConnectionStatus.Unreachable,
            Message = message
        };
    }

    private static string NormalizeHost(string host)
    {
        return (host ?? string.Empty).Trim().TrimEnd('.').ToLowerInvariant();
    }

    private static string Remove(string value, string secret)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(secret))
            return value;

        return value.Replace(secret, string.Empty, StringComparison.Ordinal);
    }
}
