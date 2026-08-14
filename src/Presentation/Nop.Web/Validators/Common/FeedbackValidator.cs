using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common;

public partial class FeedbackValidator : BaseNopValidator<FeedbackModel>
{
    public FeedbackValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Feedback.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        RuleFor(x => x.FullName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Feedback.FullName.Required"));
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Feedback.Category.Required"));
        RuleFor(x => x.Subject)
            .Must(subject => string.IsNullOrEmpty(subject) || CustomerFeedbackDefaults.Categories.Contains(subject))
            .WithMessageAwait(localizationService.GetResourceAsync("Feedback.Category.Required"));
        RuleFor(x => x.FeedbackText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Feedback.FeedbackText.Required"));
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessageAwait(localizationService.GetResourceAsync("Feedback.Rating.Required"));
    }
}
