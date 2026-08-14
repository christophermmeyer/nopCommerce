using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer feedback search model
/// </summary>
public partial record CustomerFeedbackSearchModel : BaseSearchModel
{
    #region Ctor

    public CustomerFeedbackSearchModel()
    {
        AvailableSubjects = new List<SelectListItem>();
        AvailableReadStatuses = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.List.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.List.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.List.SearchText")]
    public string SearchText { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.List.SearchSubject")]
    public string SearchSubject { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerFeedback.List.SearchIsRead")]
    public int SearchIsReadId { get; set; }

    public IList<SelectListItem> AvailableSubjects { get; set; }

    public IList<SelectListItem> AvailableReadStatuses { get; set; }

    #endregion
}
