using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common;

/// <summary>
/// Customer feedback service
/// </summary>
public partial class CustomerFeedbackService : ICustomerFeedbackService
{
    #region Fields

    protected readonly IRepository<CustomerFeedback> _customerFeedbackRepository;

    #endregion

    #region Ctor

    public CustomerFeedbackService(IRepository<CustomerFeedback> customerFeedbackRepository)
    {
        _customerFeedbackRepository = customerFeedbackRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCustomerFeedbackAsync(CustomerFeedback customerFeedback)
    {
        await _customerFeedbackRepository.DeleteAsync(customerFeedback);
    }

    /// <summary>
    /// Deletes customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCustomerFeedbackAsync(IList<CustomerFeedback> customerFeedback)
    {
        await _customerFeedbackRepository.DeleteAsync(customerFeedback);
    }

    /// <summary>
    /// Gets customer feedback by identifier
    /// </summary>
    /// <param name="customerFeedbackId">Customer feedback identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback
    /// </returns>
    public virtual async Task<CustomerFeedback> GetCustomerFeedbackByIdAsync(int customerFeedbackId)
    {
        return await _customerFeedbackRepository.GetByIdAsync(customerFeedbackId);
    }

    /// <summary>
    /// Gets customer feedback by identifiers
    /// </summary>
    /// <param name="customerFeedbackIds">Customer feedback identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback
    /// </returns>
    public virtual async Task<IList<CustomerFeedback>> GetCustomerFeedbackByIdsAsync(int[] customerFeedbackIds)
    {
        return await _customerFeedbackRepository.GetByIdsAsync(customerFeedbackIds);
    }

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
    public virtual async Task<IPagedList<CustomerFeedback>> GetAllCustomerFeedbackAsync(
        string searchText = null,
        string subject = null,
        bool? isRead = null,
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int storeId = 0,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        var query = _customerFeedbackRepository.Table;

        if (storeId > 0)
            query = query.Where(feedback => feedback.StoreId == storeId);

        if (isRead.HasValue)
            query = query.Where(feedback => feedback.IsRead == isRead.Value);

        if (createdFromUtc.HasValue)
            query = query.Where(feedback => createdFromUtc.Value <= feedback.CreatedOnUtc);

        if (createdToUtc.HasValue)
            query = query.Where(feedback => createdToUtc.Value >= feedback.CreatedOnUtc);

        if (!string.IsNullOrWhiteSpace(subject))
            query = query.Where(feedback => feedback.Subject == subject);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(feedback =>
                feedback.FullName.Contains(searchText) ||
                feedback.Email.Contains(searchText) ||
                feedback.FeedbackText.Contains(searchText));
        }

        query = query.OrderByDescending(feedback => feedback.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Inserts customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCustomerFeedbackAsync(CustomerFeedback customerFeedback)
    {
        await _customerFeedbackRepository.InsertAsync(customerFeedback);
    }

    /// <summary>
    /// Updates customer feedback
    /// </summary>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCustomerFeedbackAsync(CustomerFeedback customerFeedback)
    {
        await _customerFeedbackRepository.UpdateAsync(customerFeedback);
    }

    #endregion
}
