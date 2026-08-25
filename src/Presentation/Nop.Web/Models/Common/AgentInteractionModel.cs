using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

/// <summary>
/// Represents the public storefront agent interaction window
/// </summary>
public partial record AgentInteractionModel : BaseNopModel
{
    public AgentInteractionModel()
    {
        Messages = new List<AgentInteractionMessageModel>();
    }

    public string SendMessageUrl { get; set; }

    public IList<AgentInteractionMessageModel> Messages { get; set; }
}

/// <summary>
/// Represents a single message in the agent interaction window
/// </summary>
public partial record AgentInteractionMessageModel : BaseNopModel
{
    public string Text { get; set; }

    public bool IsFromCustomer { get; set; }

    public string CreatedOn { get; set; }
}
