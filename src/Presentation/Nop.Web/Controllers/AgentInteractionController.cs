using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class AgentInteractionController : BasePublicController
{
    #region Fields

    protected readonly IAgentInteractionService _agentInteractionService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly StoreInformationSettings _storeInformationSettings;

    #endregion

    #region Ctor

    public AgentInteractionController(IAgentInteractionService agentInteractionService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWorkContext workContext,
        StoreInformationSettings storeInformationSettings)
    {
        _agentInteractionService = agentInteractionService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _storeContext = storeContext;
        _workContext = workContext;
        _storeInformationSettings = storeInformationSettings;
    }

    #endregion

    #region Methods

    [HttpPost]
    [CheckAccessClosedStore(ignore: true)]
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SendMessage(string message)
    {
        if (!_storeInformationSettings.DisplayAgentInteractionWindow)
            return Json(new { success = false, message = await _localizationService.GetResourceAsync("AgentInteraction.Error.Generic") });

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer.IsSearchEngineAccount())
            return Json(new { success = false, message = await _localizationService.GetResourceAsync("AgentInteraction.Error.Generic") });

        var store = await _storeContext.GetCurrentStoreAsync();
        var result = await _agentInteractionService.SendMessageAsync(customer.Id, store.Id, message, store.Name);

        if (!result.Success)
        {
            return Json(new
            {
                success = false,
                message = await _localizationService.GetResourceAsync(result.ErrorResourceKey)
            });
        }

        var createdOn = await _dateTimeHelper.ConvertToUserTimeAsync(result.AgentMessage.CreatedOnUtc, DateTimeKind.Utc);

        return Json(new
        {
            success = true,
            reply = result.AgentMessage.MessageText,
            createdOn = createdOn.ToString("t")
        });
    }

    #endregion
}
