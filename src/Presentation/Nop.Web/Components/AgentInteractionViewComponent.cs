using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Common;

namespace Nop.Web.Components;

public partial class AgentInteractionViewComponent : NopViewComponent
{
    protected readonly IAgentInteractionService _agentInteractionService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly StoreInformationSettings _storeInformationSettings;

    public AgentInteractionViewComponent(IAgentInteractionService agentInteractionService,
        IDateTimeHelper dateTimeHelper,
        IStoreContext storeContext,
        IWorkContext workContext,
        StoreInformationSettings storeInformationSettings)
    {
        _agentInteractionService = agentInteractionService;
        _dateTimeHelper = dateTimeHelper;
        _storeContext = storeContext;
        _workContext = workContext;
        _storeInformationSettings = storeInformationSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_storeInformationSettings.DisplayAgentInteractionWindow)
            return Content("");

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer.IsSearchEngineAccount())
            return Content("");

        var store = await _storeContext.GetCurrentStoreAsync();
        var messages = await _agentInteractionService.GetConversationMessagesAsync(customer.Id, store.Id);
        var model = new AgentInteractionModel
        {
            SendMessageUrl = Url.RouteUrl(NopRouteNames.Ajax.AGENT_INTERACTION_SEND)
        };

        foreach (var message in messages)
        {
            model.Messages.Add(new AgentInteractionMessageModel
            {
                Text = message.MessageText,
                IsFromCustomer = message.IsFromCustomer,
                CreatedOn = (await _dateTimeHelper.ConvertToUserTimeAsync(message.CreatedOnUtc, DateTimeKind.Utc)).ToString("t")
            });
        }

        return await ViewAsync(model);
    }
}
