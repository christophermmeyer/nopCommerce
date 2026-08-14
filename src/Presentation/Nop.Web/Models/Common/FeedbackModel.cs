using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Common;

public partial record FeedbackModel : BaseNopModel
{
    public FeedbackModel()
    {
        AvailableCategories = new List<SelectListItem>();
        AvailableRatings = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Feedback.FullName")]
    public string FullName { get; set; }

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Feedback.Email")]
    public string Email { get; set; }

    [NopResourceDisplayName("Feedback.Category")]
    public string Subject { get; set; }

    [NopResourceDisplayName("Feedback.FeedbackText")]
    public string FeedbackText { get; set; }

    [NopResourceDisplayName("Feedback.Rating")]
    public int Rating { get; set; }

    public bool SuccessfullySubmitted { get; set; }
    public string Result { get; set; }
    public bool DisplayCaptcha { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }
    public IList<SelectListItem> AvailableRatings { get; set; }
}
