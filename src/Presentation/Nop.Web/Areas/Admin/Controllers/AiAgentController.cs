using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.AiAgents;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class AiAgentController : BaseAdminController
{
    private readonly IAiAgentService _aiAgentService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    public AiAgentController(IAiAgentService aiAgentService,
        ILocalizationService localizationService,
        INotificationService notificationService)
    {
        _aiAgentService = aiAgentService;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> List()
    {
        var model = new AiAgentListModel();
        foreach (var agent in await _aiAgentService.GetAllAsync())
            model.Agents.Add(await MapListItemAsync(agent));

        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create()
    {
        var model = new AiAgentModel
        {
            Role = AiAgentCatalog.Roles.ProductDescriptions,
            Provider = AiAgentCatalog.Providers.OpenAiCompatible,
            ModelName = "gpt-4.1-mini",
            EndpointUrl = "https://api.example.com/v1",
            SelectedPermissions = new List<string>
            {
                AiAgentCatalog.Permissions.ReadProductCatalog,
                AiAgentCatalog.Permissions.DraftProductDescriptions,
                AiAgentCatalog.Permissions.SuggestSeoMeta
            }
        };

        await PrepareWizardAsync(model);
        return View("Wizard", model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(Guid id)
    {
        var agent = await _aiAgentService.GetByIdAsync(id);
        if (agent == null)
        {
            _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
            return RedirectToAction(nameof(List));
        }

        var model = MapWizard(agent);
        await PrepareWizardAsync(model);
        return View("Wizard", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Save(AiAgentModel model)
    {
        if (!ModelState.IsValid)
        {
            await RedisplayWizardAsync(model);
            return View("Wizard", model);
        }

        try
        {
            var postedSecret = model.ApiKey;
            model.ApiKey = null;

            var saved = await _aiAgentService.SaveFromWizardAsync(ToDraft(model), postedSecret, model.ApiKeyConfigured);
            if (saved == null)
            {
                _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
                return RedirectToAction(nameof(List));
            }

            _notificationService.SuccessNotification(await RAsync("Admin.Configuration.AiAgents.Saved"));
            return RedirectToAction(nameof(Review), new { id = saved.Id });
        }
        catch (NopException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await RedisplayWizardAsync(model);
            return View("Wizard", model);
        }
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Review(Guid id)
    {
        var agent = await _aiAgentService.GetByIdAsync(id);
        if (agent == null)
        {
            _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
            return RedirectToAction(nameof(List));
        }

        return View(await MapReviewAsync(agent));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ConfirmReview(Guid id, bool permissionsReviewed)
    {
        if (!await _aiAgentService.SetPermissionsReviewedAsync(id, permissionsReviewed))
        {
            _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
            return RedirectToAction(nameof(List));
        }

        var key = permissionsReviewed
            ? "Admin.Configuration.AiAgents.ReviewConfirmed"
            : "Admin.Configuration.AiAgents.ReviewCleared";
        _notificationService.SuccessNotification(await RAsync(key));
        return RedirectToAction(nameof(Review), new { id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> TestConnection(Guid id)
    {
        var agent = await _aiAgentService.TestConnectionAsync(id);
        if (agent == null)
        {
            _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
            return RedirectToAction(nameof(List));
        }

        var status = (AiAgentConnectionStatus)agent.LastConnectionStatusId;
        if (status == AiAgentConnectionStatus.Success)
            _notificationService.SuccessNotification(agent.LastConnectionMessage);
        else if (status == AiAgentConnectionStatus.Unreachable)
            _notificationService.ErrorNotification(agent.LastConnectionMessage);
        else
            _notificationService.WarningNotification(agent.LastConnectionMessage);

        return RedirectToAction(nameof(Review), new { id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Enable(Guid id)
    {
        if (!await _aiAgentService.SetEnabledAsync(id, true))
        {
            var agent = await _aiAgentService.GetByIdAsync(id);
            if (agent == null)
                _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
            else
                _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.EnableBlocked"));

            return agent == null
                ? RedirectToAction(nameof(List))
                : RedirectToAction(nameof(Review), new { id });
        }

        _notificationService.SuccessNotification(await RAsync("Admin.Configuration.AiAgents.Enabled"));
        return RedirectToAction(nameof(Review), new { id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Disable(Guid id)
    {
        if (!await _aiAgentService.SetEnabledAsync(id, false))
        {
            _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
            return RedirectToAction(nameof(List));
        }

        _notificationService.SuccessNotification(await RAsync("Admin.Configuration.AiAgents.Disabled"));
        return RedirectToAction(nameof(Review), new { id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Delete(Guid id)
    {
        if (!await _aiAgentService.DeleteAsync(id))
            _notificationService.ErrorNotification(await RAsync("Admin.Configuration.AiAgents.NotFound"));
        else
            _notificationService.SuccessNotification(await RAsync("Admin.Configuration.AiAgents.Deleted"));

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> LoadExamples()
    {
        var added = await _aiAgentService.LoadExamplesAsync();
        var key = added > 0
            ? "Admin.Configuration.AiAgents.ExamplesLoaded"
            : "Admin.Configuration.AiAgents.ExamplesAlreadyLoaded";
        _notificationService.SuccessNotification(await RAsync(key));
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> RemoveAll()
    {
        await _aiAgentService.DeleteAllAsync();
        _notificationService.SuccessNotification(await RAsync("Admin.Configuration.AiAgents.RemovedAll"));
        return RedirectToAction(nameof(List));
    }

    private async Task RedisplayWizardAsync(AiAgentModel model)
    {
        var configured = AiAgentPrototypePolicy.ResolveApiKeyConfigured(model.ApiKey, model.ApiKeyConfigured);
        // ModelState would otherwise redisplay the posted placeholder in the password box.
        ModelState.Remove(nameof(AiAgentModel.ApiKey));
        model.ApiKey = null;
        model.ApiKeyConfigured = configured;
        await PrepareWizardAsync(model);
    }

    private async Task PrepareWizardAsync(AiAgentModel model)
    {
        model.AvailableRoles = await SelectAsync(AiAgentCatalog.Roles.All, "Admin.Configuration.AiAgents.Role.", model.Role);
        model.AvailableProviders = await SelectAsync(AiAgentCatalog.Providers.All, "Admin.Configuration.AiAgents.Provider.", model.Provider);
        model.PermissionChoices = await PermissionChoicesAsync(model.SelectedPermissions, includeDenied: true);
        if (model.ApiKeyConfigured)
            model.ApiKey = AiAgentCatalog.MaskedSecret;
    }

    private static AiAgentModel MapWizard(AiAgent agent)
    {
        return new AiAgentModel
        {
            Id = agent.Id,
            Name = agent.Name,
            Role = agent.Role,
            Description = agent.Description,
            Provider = agent.Provider,
            ModelName = agent.Model,
            EndpointUrl = agent.EndpointUrl,
            ApiKeyConfigured = agent.ApiKeyConfigured,
            SelectedPermissions = agent.Permissions?.ToList() ?? new List<string>()
        };
    }

    private async Task<AiAgentModel> MapListItemAsync(AiAgent agent)
    {
        var model = await MapSummaryAsync(agent);
        return model;
    }

    private async Task<AiAgentModel> MapReviewAsync(AiAgent agent)
    {
        var model = await MapSummaryAsync(agent);
        model.PermissionChoices = await ReviewRowsAsync(agent);
        model.CanEnable = AiAgentPrototypePolicy.CanEnable(agent);
        model.PermissionsReviewed = agent.PermissionsReviewed;
        model.Description = agent.Description;
        model.EndpointUrl = agent.EndpointUrl;
        model.ModelName = agent.Model;
        return model;
    }

    private async Task<AiAgentModel> MapSummaryAsync(AiAgent agent)
    {
        var display = AiAgentPrototypePolicy.GetDisplayStatus(agent);
        var connection = (AiAgentConnectionStatus)agent.LastConnectionStatusId;

        return new AiAgentModel
        {
            Id = agent.Id,
            Name = agent.Name,
            Role = agent.Role,
            RoleName = await RAsync("Admin.Configuration.AiAgents.Role." + agent.Role),
            Provider = agent.Provider,
            ProviderName = await RAsync("Admin.Configuration.AiAgents.Provider." + agent.Provider),
            ApiKeyConfigured = agent.ApiKeyConfigured,
            ApiKeyLabel = await RAsync(agent.ApiKeyConfigured
                ? "Admin.Configuration.AiAgents.ApiKey.Masked"
                : "Admin.Configuration.AiAgents.ApiKey.Missing"),
            PermissionSummary = await PermissionSummaryAsync(agent.Permissions),
            Enabled = agent.Enabled,
            PermissionsReviewed = agent.PermissionsReviewed,
            DisplayStatus = await RAsync("Admin.Configuration.AiAgents.Status." + display),
            DisplayStatusCss = DisplayCss(display),
            LastCheck = await RAsync("Admin.Configuration.AiAgents.Status." + ConnectionResource(connection)),
            LastCheckCss = ConnectionCss(connection),
            LastConnectionMessage = agent.LastConnectionMessage,
            UpdatedOn = agent.UpdatedOnUtc.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)
        };
    }

    private static AiAgent ToDraft(AiAgentModel model)
    {
        return new AiAgent
        {
            Id = model.Id,
            Name = model.Name,
            Role = model.Role,
            Description = model.Description,
            Provider = model.Provider,
            Model = model.ModelName,
            EndpointUrl = model.EndpointUrl,
            Permissions = model.SelectedPermissions?.ToList() ?? new List<string>()
        };
    }

    private async Task<IList<SelectListItem>> SelectAsync(IReadOnlyList<string> keys, string prefix, string selected)
    {
        var items = new List<SelectListItem>();
        foreach (var key in keys)
        {
            items.Add(new SelectListItem
            {
                Value = key,
                Text = await RAsync(prefix + key),
                Selected = key == selected
            });
        }

        return items;
    }

    private async Task<IList<AiAgentPermissionChoiceModel>> PermissionChoicesAsync(IList<string> selected, bool includeDenied)
    {
        var choices = new List<AiAgentPermissionChoiceModel>();
        foreach (var key in AiAgentCatalog.Permissions.Grantable)
        {
            choices.Add(new AiAgentPermissionChoiceModel
            {
                SystemName = key,
                Title = await RAsync("Admin.Configuration.AiAgents.Permission." + key),
                Notes = await RAsync("Admin.Configuration.AiAgents.Permission." + key + ".Notes"),
                Selected = selected != null && selected.Contains(key)
            });
        }

        if (!includeDenied)
            return choices;

        foreach (var key in AiAgentCatalog.Permissions.AlwaysDenied)
        {
            choices.Add(new AiAgentPermissionChoiceModel
            {
                SystemName = key,
                Title = await RAsync("Admin.Configuration.AiAgents.Permission." + key),
                Notes = await RAsync("Admin.Configuration.AiAgents.Permission." + key + ".Notes"),
                Selected = false,
                Disabled = true
            });
        }

        return choices;
    }

    private async Task<IList<AiAgentPermissionChoiceModel>> ReviewRowsAsync(AiAgent agent)
    {
        var rows = new List<AiAgentPermissionChoiceModel>();
        var allowed = await RAsync("Admin.Configuration.AiAgents.Access.Allowed");
        var denied = await RAsync("Admin.Configuration.AiAgents.Access.Denied");

        foreach (var key in AiAgentCatalog.Permissions.Grantable)
        {
            var selected = agent.Permissions != null && agent.Permissions.Contains(key);
            rows.Add(new AiAgentPermissionChoiceModel
            {
                SystemName = key,
                Title = await RAsync("Admin.Configuration.AiAgents.Permission." + key),
                Notes = await RAsync("Admin.Configuration.AiAgents.Permission." + key + ".Notes"),
                Selected = selected,
                Access = selected ? allowed : denied
            });
        }

        foreach (var key in AiAgentCatalog.Permissions.AlwaysDenied)
        {
            rows.Add(new AiAgentPermissionChoiceModel
            {
                SystemName = key,
                Title = await RAsync("Admin.Configuration.AiAgents.Permission." + key),
                Notes = await RAsync("Admin.Configuration.AiAgents.Permission." + key + ".Notes"),
                Selected = false,
                Disabled = true,
                Access = denied
            });
        }

        return rows;
    }

    private async Task<string> PermissionSummaryAsync(IList<string> permissions)
    {
        var names = new List<string>();
        if (permissions != null)
        {
            foreach (var key in AiAgentCatalog.Permissions.Grantable)
            {
                if (permissions.Contains(key))
                    names.Add(await RAsync("Admin.Configuration.AiAgents.Permission." + key));
            }
        }

        names.Add(await RAsync("Admin.Configuration.AiAgents.NoAutoPublish"));
        return string.Join(" · ", names);
    }

    private async Task<string> RAsync(string key)
    {
        var value = await _localizationService.GetResourceAsync(key);
        return string.IsNullOrEmpty(value) ? key : value;
    }

    private static string DisplayCss(AiAgentDisplayStatus status)
    {
        return status switch
        {
            AiAgentDisplayStatus.Enabled => "badge-success",
            AiAgentDisplayStatus.NeedsReview => "badge-warning",
            _ => "badge-secondary"
        };
    }

    private static string ConnectionCss(AiAgentConnectionStatus status)
    {
        return status switch
        {
            AiAgentConnectionStatus.Success => "badge-success",
            AiAgentConnectionStatus.Misconfigured => "badge-warning",
            AiAgentConnectionStatus.Unreachable => "badge-danger",
            _ => "badge-secondary"
        };
    }

    private static string ConnectionResource(AiAgentConnectionStatus status)
    {
        return status switch
        {
            AiAgentConnectionStatus.Success => "Success",
            AiAgentConnectionStatus.Misconfigured => "Misconfigured",
            AiAgentConnectionStatus.Unreachable => "Unreachable",
            _ => "NotTested"
        };
    }
}
