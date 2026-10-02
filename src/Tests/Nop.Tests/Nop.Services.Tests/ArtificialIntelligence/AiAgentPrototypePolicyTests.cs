using AwesomeAssertions;
using Newtonsoft.Json;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.ArtificialIntelligence;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ArtificialIntelligence;

[TestFixture]
public class AiAgentPrototypePolicyTests
{
    [Test]
    public void PlaceholderEndpointIsMisconfiguredNotDead()
    {
        var result = AiAgentPrototypePolicy.Test(Agent("https://api.example.com/v1", "gpt-4.1-mini", configured: true));

        result.Status.Should().Be(AiAgentConnectionStatus.Misconfigured);
        result.Message.Should().Contain("configuration problem");
        result.Message.Should().NotContain("dead agent");
    }

    [Test]
    public void InvalidHostIsUnreachableNotMisconfigured()
    {
        var result = AiAgentPrototypePolicy.Test(Agent("https://unreachable.invalid/v1", "gpt-4.1-mini", configured: true));

        result.Status.Should().Be(AiAgentConnectionStatus.Unreachable);
        result.Message.Should().Contain("dead agent");
        result.Message.Should().NotContain("configuration problem");
    }

    [Test]
    public void MissingKeyIsMisconfiguredEvenWithCompleteUrl()
    {
        var result = AiAgentPrototypePolicy.Test(Agent("https://catalog-assistant.test/v1", "gpt-4.1-mini", configured: false));

        result.Status.Should().Be(AiAgentConnectionStatus.Misconfigured);
        result.Message.Should().Contain("API key");
    }

    [Test]
    public void CompletePlaceholderEndpointPassesWithoutContactingAProvider()
    {
        var result = AiAgentPrototypePolicy.Test(Agent("https://catalog-assistant.test/v1", "gpt-4.1-mini", configured: true));

        result.Status.Should().Be(AiAgentConnectionStatus.Success);
        result.Message.Should().Contain("No live provider was contacted");
    }

    [Test]
    public void UnreachableModelNameIsADeadAgentWhenTheUrlLooksComplete()
    {
        var result = AiAgentPrototypePolicy.Test(Agent("https://catalog-assistant.test/v1", "unreachable", configured: true));

        result.Status.Should().Be(AiAgentConnectionStatus.Unreachable);
    }

    [Test]
    public void EnableRequiresReviewAndASuccessfulCheck()
    {
        var agent = Agent("https://catalog-assistant.test/v1", "gpt-4.1-mini", configured: true);
        agent.PermissionsReviewed = true;
        agent.LastConnectionStatusId = (int)AiAgentConnectionStatus.Success;
        AiAgentPrototypePolicy.CanEnable(agent).Should().BeTrue();

        agent.PermissionsReviewed = false;
        AiAgentPrototypePolicy.CanEnable(agent).Should().BeFalse();

        agent.PermissionsReviewed = true;
        agent.LastConnectionStatusId = (int)AiAgentConnectionStatus.Misconfigured;
        AiAgentPrototypePolicy.CanEnable(agent).Should().BeFalse();

        agent.LastConnectionStatusId = (int)AiAgentConnectionStatus.Unreachable;
        AiAgentPrototypePolicy.CanEnable(agent).Should().BeFalse();

        agent.LastConnectionStatusId = (int)AiAgentConnectionStatus.NotTested;
        AiAgentPrototypePolicy.CanEnable(agent).Should().BeFalse();
    }

    [Test]
    public void FailedCheckTurnsTheAgentOff()
    {
        var agent = Agent("https://unreachable.invalid/v1", "gpt-4.1-mini", configured: true);
        agent.Enabled = true;
        agent.PermissionsReviewed = true;

        AiAgentPrototypePolicy.ApplyTestResult(agent, AiAgentPrototypePolicy.Test(agent), DateTime.UtcNow);

        agent.Enabled.Should().BeFalse();
        agent.LastConnectionStatusId.Should().Be((int)AiAgentConnectionStatus.Unreachable);
    }

    [Test]
    public void WizardSaveClearsEnableReviewAndLastCheck()
    {
        var agent = Agent("https://catalog-assistant.test/v1", "gpt-4.1-mini", configured: true);
        agent.Enabled = true;
        agent.PermissionsReviewed = true;
        agent.LastConnectionStatusId = (int)AiAgentConnectionStatus.Success;
        agent.LastConnectionMessage = "ok";

        AiAgentPrototypePolicy.MarkNeedsReview(agent);

        agent.Enabled.Should().BeFalse();
        agent.PermissionsReviewed.Should().BeFalse();
        agent.LastConnectionStatusId.Should().Be((int)AiAgentConnectionStatus.NotTested);
        agent.LastConnectionMessage.Should().BeNull();
        AiAgentPrototypePolicy.GetDisplayStatus(agent).Should().Be(AiAgentDisplayStatus.NeedsReview);
    }

    [Test]
    public void PostedSecretIsNotStored()
    {
        const string secret = "sk-prototype-should-not-persist";
        var agent = Agent("https://catalog-assistant.test/v1", "gpt-4.1-mini", configured: false);
        agent.Description = "notes " + secret;

        AiAgentPrototypePolicy.ScrubPostedSecret(agent, secret);
        agent.ApiKeyConfigured = AiAgentPrototypePolicy.ResolveApiKeyConfigured(secret, alreadyConfigured: false);

        agent.ApiKeyConfigured.Should().BeTrue();
        agent.Description.Should().NotContain(secret);
        JsonConvert.SerializeObject(agent).Should().NotContain(secret);
    }

    [Test]
    public void MaskKeepsThePreviousFlagAndEmptyClearsIt()
    {
        AiAgentPrototypePolicy.ResolveApiKeyConfigured(AiAgentCatalog.MaskedSecret, alreadyConfigured: true).Should().BeTrue();
        AiAgentPrototypePolicy.ResolveApiKeyConfigured(AiAgentCatalog.MaskedSecret, alreadyConfigured: false).Should().BeFalse();
        AiAgentPrototypePolicy.ResolveApiKeyConfigured("   ", alreadyConfigured: true).Should().BeFalse();
        AiAgentPrototypePolicy.ResolveApiKeyConfigured("placeholder", alreadyConfigured: false).Should().BeTrue();
    }

    [Test]
    public void PublishAndPaymentPermissionsCannotBeGranted()
    {
        var normalized = AiAgentPrototypePolicy.NormalizePermissions(new[]
        {
            AiAgentCatalog.Permissions.ReadProductCatalog,
            AiAgentCatalog.Permissions.PublishWithoutReview,
            AiAgentCatalog.Permissions.ReadPaymentDetails,
            AiAgentCatalog.Permissions.ReadCustomerPii,
            "NotARealPermission"
        });

        normalized.Should().BeEquivalentTo([AiAgentCatalog.Permissions.ReadProductCatalog]);
        normalized.Should().NotContain(AiAgentCatalog.Permissions.PublishWithoutReview);
        normalized.Should().NotContain(AiAgentCatalog.Permissions.ReadPaymentDetails);
    }

    [Test]
    public void ReviewedButOffAgentDisplaysAsDisabled()
    {
        var agent = Agent("https://catalog-assistant.test/v1", "gpt-4.1-mini", configured: true);
        agent.PermissionsReviewed = true;
        agent.Enabled = false;

        AiAgentPrototypePolicy.GetDisplayStatus(agent).Should().Be(AiAgentDisplayStatus.Disabled);
    }

    private static AiAgent Agent(string endpoint, string model, bool configured)
    {
        return new AiAgent
        {
            Model = model,
            EndpointUrl = endpoint,
            ApiKeyConfigured = configured
        };
    }
}
