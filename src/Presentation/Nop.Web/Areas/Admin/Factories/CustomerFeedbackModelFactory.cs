using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer feedback model factory implementation
/// </summary>
public partial class CustomerFeedbackModelFactory : ICustomerFeedbackModelFactory
{
    #region Fields

    protected readonly ICustomerFeedbackService _customerFeedbackService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public CustomerFeedbackModelFactory(ICustomerFeedbackService customerFeedbackService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IStoreService storeService)
    {
        _customerFeedbackService = customerFeedbackService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    protected virtual async Task<CustomerFeedbackModel> PrepareModelAsync(CustomerFeedback customerFeedback)
    {
        var store = await _storeService.GetStoreByIdAsync(customerFeedback.StoreId);
        var customer = await _customerService.GetCustomerByIdAsync(customerFeedback.CustomerId);

        return new CustomerFeedbackModel
        {
            Id = customerFeedback.Id,
            StoreId = customerFeedback.StoreId,
            StoreName = store?.Name ?? string.Empty,
            CustomerId = customerFeedback.CustomerId,
            CustomerEmail = customer?.Email ?? string.Empty,
            FullName = customerFeedback.FullName,
            Email = customerFeedback.Email,
            Subject = customerFeedback.Subject,
            SubjectName = await _localizationService.GetResourceAsync($"Feedback.Category.{customerFeedback.Subject}"),
            FeedbackText = customerFeedback.FeedbackText,
            Rating = customerFeedback.Rating,
            IsRead = customerFeedback.IsRead,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customerFeedback.CreatedOnUtc, DateTimeKind.Utc)
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare customer feedback search model
    /// </summary>
    /// <param name="searchModel">Customer feedback search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback search model
    /// </returns>
    public virtual async Task<CustomerFeedbackSearchModel> PrepareCustomerFeedbackSearchModelAsync(CustomerFeedbackSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.AvailableSubjects.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
            Value = string.Empty
        });

        foreach (var category in CustomerFeedbackDefaults.Categories)
        {
            searchModel.AvailableSubjects.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync($"Feedback.Category.{category}"),
                Value = category
            });
        }

        searchModel.AvailableReadStatuses.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
            Value = "0"
        });
        searchModel.AvailableReadStatuses.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Common.Yes"),
            Value = "1"
        });
        searchModel.AvailableReadStatuses.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Common.No"),
            Value = "2"
        });

        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged customer feedback list model
    /// </summary>
    /// <param name="searchModel">Customer feedback search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback list model
    /// </returns>
    public virtual async Task<CustomerFeedbackListModel> PrepareCustomerFeedbackListModelAsync(CustomerFeedbackSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        var createdOnFromValue = searchModel.CreatedOnFrom.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
            : null;
        var createdOnToValue = searchModel.CreatedOnTo.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
            : null;
        bool? isRead = searchModel.SearchIsReadId switch
        {
            1 => true,
            2 => false,
            _ => null
        };

        var feedbackItems = await _customerFeedbackService.GetAllCustomerFeedbackAsync(
            searchText: searchModel.SearchText,
            subject: searchModel.SearchSubject,
            isRead: isRead,
            createdFromUtc: createdOnFromValue,
            createdToUtc: createdOnToValue,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        var model = await new CustomerFeedbackListModel().PrepareToGridAsync(searchModel, feedbackItems, () =>
        {
            return feedbackItems.SelectAwait(async feedback => await PrepareModelAsync(feedback));
        });

        return model;
    }

    /// <summary>
    /// Prepare customer feedback model
    /// </summary>
    /// <param name="model">Customer feedback model</param>
    /// <param name="customerFeedback">Customer feedback</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer feedback model
    /// </returns>
    public virtual async Task<CustomerFeedbackModel> PrepareCustomerFeedbackModelAsync(CustomerFeedbackModel model, CustomerFeedback customerFeedback)
    {
        ArgumentNullException.ThrowIfNull(customerFeedback);

        return model ?? await PrepareModelAsync(customerFeedback);
    }

    #endregion
}
