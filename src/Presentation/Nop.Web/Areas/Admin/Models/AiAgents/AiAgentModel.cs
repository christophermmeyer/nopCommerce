using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.AiAgents;

public partial record AiAgentModel : BaseNopModel
{
    public AiAgentModel()
    {
        SelectedPermissions = new List<string>();
        AvailableRoles = new List<SelectListItem>();
        AvailableProviders = new List<SelectListItem>();
        PermissionChoices = new List<AiAgentPermissionChoiceModel>();
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public string RoleName { get; set; }

    public string Description { get; set; }

    public string Provider { get; set; }

    public string ProviderName { get; set; }

    public string ModelName { get; set; }

    public string EndpointUrl { get; set; }

    /// <summary>
    /// Posted placeholder only. Cleared before the agent is stored.
    /// </summary>
    public string ApiKey { get; set; }

    public bool ApiKeyConfigured { get; set; }

    public string ApiKeyLabel { get; set; }

    public IList<string> SelectedPermissions { get; set; }

    public string PermissionSummary { get; set; }

    public bool PermissionsReviewed { get; set; }

    public bool Enabled { get; set; }

    public bool CanEnable { get; set; }

    public string DisplayStatus { get; set; }

    public string DisplayStatusCss { get; set; }

    public string LastCheck { get; set; }

    public string LastCheckCss { get; set; }

    public string LastConnectionMessage { get; set; }

    public string UpdatedOn { get; set; }

    public IList<SelectListItem> AvailableRoles { get; set; }

    public IList<SelectListItem> AvailableProviders { get; set; }

    public IList<AiAgentPermissionChoiceModel> PermissionChoices { get; set; }
}
