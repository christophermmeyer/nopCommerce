using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer feedback list model
/// </summary>
public partial record CustomerFeedbackListModel : BasePagedListModel<CustomerFeedbackModel>;
