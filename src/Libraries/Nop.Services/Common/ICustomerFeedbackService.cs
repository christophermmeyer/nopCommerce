using Nop.Core;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common;

/// <summary>
/// Customer feedback service interface
/// </summary>
public partial interface ICustomerFeedbackService
{
    /// <summary>
    /// Deletes customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteCustomerFeedbackAsync(CustomerFeedback customerFeedback);

    /// <summary>
    /// Deletes customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteCustomerFeedbackAsync(IList<CustomerFeedback> customerFeedback);

    /// <summary>
    /// Gets customer feedback by identifier
    /// </summary>
    /// <param name="customerFeedbackId">Customer feedback identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback
    /// </returns>
    Task<CustomerFeedback> GetCustomerFeedbackByIdAsync(int customerFeedbackId);

    /// <summary>
    /// Gets customer feedback by identifiers
    /// </summary>
    /// <param name="customerFeedbackIds">Customer feedback identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback
    /// </returns>
    Task<IList<CustomerFeedback>> GetCustomerFeedbackByIdsAsync(int[] customerFeedbackIds);

    /// <summary>
    /// Gets all customer feedback
    /// </summary>
    /// <param name="searchText">Search text (name, email, or feedback)</param>
    /// <param name="subject">Category; pass null or empty to load all records</param>
    /// <param name="isRead">Read status; pass null to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback
    /// </returns>
    Task<IPagedList<CustomerFeedback>> GetAllCustomerFeedbackAsync(
        string searchText = null,
        string subject = null,
        bool? isRead = null,
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int storeId = 0,
        int pageIndex = 0,
        int pageSize = int.MaxValue);

    /// <summary>
    /// Inserts customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertCustomerFeedbackAsync(CustomerFeedback customerFeedback);

    /// <summary>
    /// Updates customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateCustomerFeedbackAsync(CustomerFeedback customerFeedback);
}
