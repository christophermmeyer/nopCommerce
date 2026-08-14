using Nop.Core.Domain.Common;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer feedback model factory
/// </summary>
public partial interface ICustomerFeedbackModelFactory
{
    /// <summary>
    /// Prepare customer feedback search model
    /// </summary>
    /// <param name="searchModel">Customer feedback search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback search model
    /// </returns>
    Task<CustomerFeedbackSearchModel> PrepareCustomerFeedbackSearchModelAsync(CustomerFeedbackSearchModel searchModel);

    /// <summary>
    /// Prepare paged customer feedback list model
    /// </summary>
    /// <param name="searchModel">Customer feedback search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback list model
    /// </returns>
    Task<CustomerFeedbackListModel> PrepareCustomerFeedbackListModelAsync(CustomerFeedbackSearchModel searchModel);

    /// <summary>
    /// Prepare customer feedback model
    /// </summary>
    /// <param name="model">Customer feedback model</param>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback model
    /// </returns>
    Task<CustomerFeedbackModel> PrepareCustomerFeedbackModelAsync(CustomerFeedbackModel model, CustomerFeedback customerFeedback);
}
