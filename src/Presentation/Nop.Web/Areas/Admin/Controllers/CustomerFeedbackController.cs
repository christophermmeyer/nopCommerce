using Microsoft.AspNetCore.Mvc;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CustomerFeedbackController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerFeedbackModelFactory _customerFeedbackModelFactory;
    protected readonly ICustomerFeedbackService _customerFeedbackService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;

    #endregion

    #region Ctor

    public CustomerFeedbackController(ICustomerActivityService customerActivityService,
        ICustomerFeedbackModelFactory customerFeedbackModelFactory,
        ICustomerFeedbackService customerFeedbackService,
        ILocalizationService localizationService,
        INotificationService notificationService)
    {
        _customerActivityService = customerActivityService;
        _customerFeedbackModelFactory = customerFeedbackModelFactory;
        _customerFeedbackService = customerFeedbackService;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        var model = await _customerFeedbackModelFactory.PrepareCustomerFeedbackSearchModelAsync(new CustomerFeedbackSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> CustomerFeedbackList(CustomerFeedbackSearchModel searchModel)
    {
        var model = await _customerFeedbackModelFactory.PrepareCustomerFeedbackListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> View(int id)
    {
        var customerFeedback = await _customerFeedbackService.GetCustomerFeedbackByIdAsync(id);
        if (customerFeedback == null)
            return RedirectToAction("List");

        var model = await _customerFeedbackModelFactory.PrepareCustomerFeedbackModelAsync(null, customerFeedback);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MarkAsRead(int id)
    {
        var customerFeedback = await _customerFeedbackService.GetCustomerFeedbackByIdAsync(id);
        if (customerFeedback == null)
            return RedirectToAction("List");

        if (!customerFeedback.IsRead)
        {
            customerFeedback.IsRead = true;
            await _customerFeedbackService.UpdateCustomerFeedbackAsync(customerFeedback);

            await _customerActivityService.InsertActivityAsync("EditCustomerFeedback",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomerFeedback"), customerFeedback.Id),
                customerFeedback);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerFeedback.MarkedAsRead"));

        return RedirectToAction("View", new { id = customerFeedback.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var customerFeedback = await _customerFeedbackService.GetCustomerFeedbackByIdAsync(id);
        if (customerFeedback == null)
            return RedirectToAction("List");

        await _customerFeedbackService.DeleteCustomerFeedbackAsync(customerFeedback);

        await _customerActivityService.InsertActivityAsync("DeleteCustomerFeedback",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerFeedback"), customerFeedback.Id),
            customerFeedback);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerFeedback.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var customerFeedback = await _customerFeedbackService.GetCustomerFeedbackByIdsAsync([.. selectedIds]);
        await _customerFeedbackService.DeleteCustomerFeedbackAsync(customerFeedback);

        await _customerActivityService.InsertActivityAsync("DeleteCustomerFeedback",
            await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerFeedback"));

        return Json(new { Result = true });
    }

    #endregion
}
