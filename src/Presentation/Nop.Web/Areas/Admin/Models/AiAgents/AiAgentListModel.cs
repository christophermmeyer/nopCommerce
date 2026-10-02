using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.AiAgents;

public partial record AiAgentListModel : BaseNopModel
{
    public AiAgentListModel()
    {
        Agents = new List<AiAgentModel>();
    }

    public IList<AiAgentModel> Agents { get; set; }
}
