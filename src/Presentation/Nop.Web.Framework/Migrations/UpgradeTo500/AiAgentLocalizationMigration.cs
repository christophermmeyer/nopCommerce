using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

/// <summary>
/// Locale resources for the admin AI agent prototype. Fresh installs load the same strings from defaultResources.nopres.xml.
/// </summary>
[NopMigration("2026-10-02 18:00:00", "MEYER-53 AI agent admin locale resources", MigrationProcessType.Update)]
public class AiAgentLocalizationMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Admin.Configuration.AiAgents"] = "AI agents",
            ["Admin.Configuration.AiAgents.AddNew"] = "Add agent",
            ["Admin.Configuration.AiAgents.BackToList"] = "back to AI agent list",
            ["Admin.Configuration.AiAgents.Edit"] = "Edit agent",
            ["Admin.Configuration.AiAgents.Intro"] = "Connect assistants that help with catalog content, support drafts, and admin workflows. Customer-facing pages never receive provider keys.",
            ["Admin.Configuration.AiAgents.PrototypeBanner"] = "Prototype. Connection fields stay masked. No live third-party credentials are stored or sent.",
            ["Admin.Configuration.AiAgents.Callout"] = "Agents run only in admin contexts you enable. Customer-facing pages never receive provider keys.",
            ["Admin.Configuration.AiAgents.CheckLegend"] = "Misconfigured means the settings are incomplete or still a placeholder. Unreachable means the settings look complete but the endpoint is treated as a dead agent. Neither check contacts a live provider.",
            ["Admin.Configuration.AiAgents.Empty.Title"] = "No AI agents yet",
            ["Admin.Configuration.AiAgents.Empty.Text"] = "Add an agent to draft catalog copy, summarize orders for support, or generate SEO fields — with permissions you control.",
            ["Admin.Configuration.AiAgents.Empty.AddFirst"] = "Add your first agent",
            ["Admin.Configuration.AiAgents.LoadExamples"] = "Load example agents",
            ["Admin.Configuration.AiAgents.ExamplesLoaded"] = "Example agents loaded. No credentials were stored.",
            ["Admin.Configuration.AiAgents.ExamplesAlreadyLoaded"] = "Example agents are already in the list.",
            ["Admin.Configuration.AiAgents.RemoveAll"] = "Remove all agents",
            ["Admin.Configuration.AiAgents.RemovedAll"] = "All AI agents were removed.",
            ["Admin.Configuration.AiAgents.RemoveAllConfirm"] = "Remove every AI agent from this prototype list?",
            ["Admin.Configuration.AiAgents.Saved"] = "Agent saved and left disabled. Review permissions, then run Test connection before enable.",
            ["Admin.Configuration.AiAgents.Deleted"] = "The agent has been deleted.",
            ["Admin.Configuration.AiAgents.Enabled"] = "The agent is enabled.",
            ["Admin.Configuration.AiAgents.Disabled"] = "The agent is disabled.",
            ["Admin.Configuration.AiAgents.ReviewConfirmed"] = "Permissions review recorded. Enable stays locked until Test connection succeeds.",
            ["Admin.Configuration.AiAgents.ReviewCleared"] = "Permissions review cleared. The agent cannot be enabled.",
            ["Admin.Configuration.AiAgents.EnableBlocked"] = "Enable stays locked until permissions are reviewed and Test connection succeeds.",
            ["Admin.Configuration.AiAgents.NotFound"] = "Agent was not found.",
            ["Admin.Configuration.AiAgents.ConfiguredAgents"] = "Configured agents",
            ["Admin.Configuration.AiAgents.Fields.Name"] = "Display name",
            ["Admin.Configuration.AiAgents.Fields.Name.Hint"] = "Shown in admin lists and permission reviews.",
            ["Admin.Configuration.AiAgents.Fields.Name.Required"] = "Display name is required.",
            ["Admin.Configuration.AiAgents.Fields.Name.MaxLength"] = "Display name must be 80 characters or fewer.",
            ["Admin.Configuration.AiAgents.Fields.Role"] = "Primary role",
            ["Admin.Configuration.AiAgents.Fields.Description"] = "Description",
            ["Admin.Configuration.AiAgents.Fields.Description.Hint"] = "What this agent is for. Publishing stays with a human admin.",
            ["Admin.Configuration.AiAgents.Fields.Provider"] = "Provider",
            ["Admin.Configuration.AiAgents.Fields.Model"] = "Model",
            ["Admin.Configuration.AiAgents.Fields.Model.Hint"] = "Model id. This prototype does not call the provider.",
            ["Admin.Configuration.AiAgents.Fields.ApiKey"] = "API key",
            ["Admin.Configuration.AiAgents.Fields.ApiKey.Hint"] = "Type any placeholder to mark a key configured. The value is discarded and never stored.",
            ["Admin.Configuration.AiAgents.Fields.Endpoint"] = "Endpoint URL",
            ["Admin.Configuration.AiAgents.Fields.Endpoint.Hint"] = "https://api.example.com/v1 stays misconfigured. https://unreachable.invalid/v1 is a dead agent. Any other absolute https URL, such as https://catalog-assistant.test/v1, passes the prototype check.",
            ["Admin.Configuration.AiAgents.Fields.Status"] = "Status",
            ["Admin.Configuration.AiAgents.Fields.Updated"] = "Updated",
            ["Admin.Configuration.AiAgents.Fields.LastCheck"] = "Last check",
            ["Admin.Configuration.AiAgents.ConnectionWarning"] = "Do not paste production secrets. This prototype keeps a configured flag only. The key itself is not stored.",
            ["Admin.Configuration.AiAgents.PermissionsIntro"] = "Choose what this agent may read or draft. Publishing always stays with a human admin.",
            ["Admin.Configuration.AiAgents.PublishDisabledHint"] = "Publish without review stays off and cannot be granted.",
            ["Admin.Configuration.AiAgents.SaveHint"] = "Save keeps the agent disabled until you review permissions and Test connection succeeds.",
            ["Admin.Configuration.AiAgents.Wizard.Basics"] = "1. Basics",
            ["Admin.Configuration.AiAgents.Wizard.BasicsHint"] = "Name and role",
            ["Admin.Configuration.AiAgents.Wizard.Connection"] = "2. Connection",
            ["Admin.Configuration.AiAgents.Wizard.ConnectionHint"] = "Provider, no real keys",
            ["Admin.Configuration.AiAgents.Wizard.Permissions"] = "3. Permissions",
            ["Admin.Configuration.AiAgents.Wizard.PermissionsHint"] = "What it can touch",
            ["Admin.Configuration.AiAgents.Wizard.Review"] = "4. Review",
            ["Admin.Configuration.AiAgents.Wizard.ReviewHint"] = "Confirm and save",
            ["Admin.Configuration.AiAgents.Steps.BasicsTitle"] = "Step 1 — Basics",
            ["Admin.Configuration.AiAgents.Steps.ConnectionTitle"] = "Step 2 — Connection",
            ["Admin.Configuration.AiAgents.Steps.PermissionsTitle"] = "Step 3 — Permissions",
            ["Admin.Configuration.AiAgents.Steps.ReviewTitle"] = "Step 4 — Review and save",
            ["Admin.Configuration.AiAgents.Review.Title"] = "Permissions",
            ["Admin.Configuration.AiAgents.Review.Confirm"] = "I have reviewed these permissions",
            ["Admin.Configuration.AiAgents.Review.ConfirmHint"] = "Recording a review does not enable the agent.",
            ["Admin.Configuration.AiAgents.Review.Save"] = "Save review",
            ["Admin.Configuration.AiAgents.TestConnection"] = "Test connection",
            ["Admin.Configuration.AiAgents.Enable"] = "Enable agent",
            ["Admin.Configuration.AiAgents.Disable"] = "Disable agent",
            ["Admin.Configuration.AiAgents.Reveal"] = "Reveal",
            ["Admin.Configuration.AiAgents.ApiKey.Masked"] = "Configured (masked)",
            ["Admin.Configuration.AiAgents.ApiKey.Missing"] = "Not configured",
            ["Admin.Configuration.AiAgents.Permissions.Column.Capability"] = "Capability",
            ["Admin.Configuration.AiAgents.Permissions.Column.Access"] = "Access",
            ["Admin.Configuration.AiAgents.Permissions.Column.Notes"] = "Notes",
            ["Admin.Configuration.AiAgents.Access.Allowed"] = "Allowed",
            ["Admin.Configuration.AiAgents.Access.Denied"] = "Denied",
            ["Admin.Configuration.AiAgents.Status.Enabled"] = "Enabled",
            ["Admin.Configuration.AiAgents.Status.Disabled"] = "Disabled",
            ["Admin.Configuration.AiAgents.Status.NeedsReview"] = "Needs review",
            ["Admin.Configuration.AiAgents.Status.NotTested"] = "Not tested",
            ["Admin.Configuration.AiAgents.Status.Success"] = "OK",
            ["Admin.Configuration.AiAgents.Status.Misconfigured"] = "Misconfigured",
            ["Admin.Configuration.AiAgents.Status.Unreachable"] = "Unreachable",
            ["Admin.Configuration.AiAgents.NoAutoPublish"] = "No auto-publish",
            ["Admin.Configuration.AiAgents.Role.ProductDescriptions"] = "Product descriptions",
            ["Admin.Configuration.AiAgents.Role.CustomerServiceDrafts"] = "Customer service drafts",
            ["Admin.Configuration.AiAgents.Role.MetaTitlesAndDescriptions"] = "Meta titles & descriptions",
            ["Admin.Configuration.AiAgents.Role.CategorySummaries"] = "Category summaries",
            ["Admin.Configuration.AiAgents.Role.Custom"] = "Custom (advanced)",
            ["Admin.Configuration.AiAgents.Provider.OpenAiCompatible"] = "OpenAI-compatible",
            ["Admin.Configuration.AiAgents.Provider.Anthropic"] = "Anthropic",
            ["Admin.Configuration.AiAgents.Provider.AzureOpenAi"] = "Azure OpenAI",
            ["Admin.Configuration.AiAgents.Provider.CustomEndpoint"] = "Custom endpoint",
            ["Admin.Configuration.AiAgents.Permission.ReadProductCatalog"] = "Read product catalog",
            ["Admin.Configuration.AiAgents.Permission.ReadProductCatalog.Notes"] = "Attributes, existing descriptions, categories",
            ["Admin.Configuration.AiAgents.Permission.DraftProductDescriptions"] = "Draft product descriptions",
            ["Admin.Configuration.AiAgents.Permission.DraftProductDescriptions.Notes"] = "Creates drafts only; admin must publish",
            ["Admin.Configuration.AiAgents.Permission.DraftCategoryCopy"] = "Draft category copy",
            ["Admin.Configuration.AiAgents.Permission.DraftCategoryCopy.Notes"] = "Drafts category text; admin must publish",
            ["Admin.Configuration.AiAgents.Permission.SuggestSeoMeta"] = "Suggest SEO meta fields",
            ["Admin.Configuration.AiAgents.Permission.SuggestSeoMeta.Notes"] = "Meta title and description suggestions",
            ["Admin.Configuration.AiAgents.Permission.ReadCustomerMessages"] = "Read customer messages",
            ["Admin.Configuration.AiAgents.Permission.ReadCustomerMessages.Notes"] = "Message text only. This does not grant customer PII fields",
            ["Admin.Configuration.AiAgents.Permission.DraftSupportReplies"] = "Draft support replies",
            ["Admin.Configuration.AiAgents.Permission.DraftSupportReplies.Notes"] = "Creates drafts only; admin must send",
            ["Admin.Configuration.AiAgents.Permission.ReadOrderSummaries"] = "Read orders (summary only)",
            ["Admin.Configuration.AiAgents.Permission.ReadOrderSummaries.Notes"] = "Order totals and status, not payment data",
            ["Admin.Configuration.AiAgents.Permission.ReadCustomerPii"] = "Read customer PII",
            ["Admin.Configuration.AiAgents.Permission.ReadCustomerPii.Notes"] = "Blocks names, emails, addresses",
            ["Admin.Configuration.AiAgents.Permission.ReadPaymentDetails"] = "Read payment details",
            ["Admin.Configuration.AiAgents.Permission.ReadPaymentDetails.Notes"] = "Never granted to agents",
            ["Admin.Configuration.AiAgents.Permission.PublishWithoutReview"] = "Publish without review",
            ["Admin.Configuration.AiAgents.Permission.PublishWithoutReview.Notes"] = "Hard default. Requires a separate policy change"
        });
    }

    public override void Down()
    {
    }
}
