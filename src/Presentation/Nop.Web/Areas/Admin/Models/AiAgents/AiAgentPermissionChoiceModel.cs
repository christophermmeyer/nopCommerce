using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.AiAgents;

public partial record AiAgentPermissionChoiceModel : BaseNopModel
{
    public string SystemName { get; set; }

    public string Title { get; set; }

    public string Notes { get; set; }

    public bool Selected { get; set; }

    public bool Disabled { get; set; }

    public string Access { get; set; }
}
