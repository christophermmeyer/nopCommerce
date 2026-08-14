using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer feedback model
/// </summary>
public partial record CustomerFeedbackModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Store")]
    public int StoreId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Store")]
    public string StoreName { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Customer")]
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Customer")]
    public string CustomerEmail { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.FullName")]
    public string FullName { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Email")]
    public string Email { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Subject")]
    public string Subject { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Subject")]
    public string SubjectName { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.FeedbackText")]
    public string FeedbackText { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.Rating")]
    public int Rating { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.IsRead")]
    public bool IsRead { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }
}
