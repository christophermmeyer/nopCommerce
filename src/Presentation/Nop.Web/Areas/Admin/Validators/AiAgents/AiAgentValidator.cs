using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.AiAgents;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.AiAgents;

public partial class AiAgentValidator : BaseNopValidator<AiAgentModel>
{
    public AiAgentValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.AiAgents.Fields.Name.Required"));

        RuleFor(model => model.Name)
            .MaximumLength(80)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.AiAgents.Fields.Name.MaxLength"));
    }
}
