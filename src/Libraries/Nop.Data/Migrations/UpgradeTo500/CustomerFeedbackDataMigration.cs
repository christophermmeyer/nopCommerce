using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-08-14 20:00:01", "5.00", UpdateMigrationType.Data)]
public class CustomerFeedbackDataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public CustomerFeedbackDataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "PublicStore.SubmitFeedback", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SubmitFeedback",
                    Enabled = true,
                    Name = "Public store. Submit feedback"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteCustomerFeedback", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerFeedback",
                    Enabled = true,
                    Name = "Delete customer feedback"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditCustomerFeedback", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerFeedback",
                    Enabled = true,
                    Name = "Edit customer feedback"
                }
            );
        }

        var resources = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["ActivityLog.PublicStore.SubmitFeedback"] = "Submitted store feedback",
            ["ActivityLog.DeleteCustomerFeedback"] = "Deleted a customer feedback (ID = {0})",
            ["ActivityLog.EditCustomerFeedback"] = "Edited a customer feedback (ID = {0})",
            ["Admin.Customers.CustomerFeedback"] = "Customer feedback",
            ["Admin.Customers.CustomerFeedback.BackToList"] = "back to customer feedback list",
            ["Admin.Customers.CustomerFeedback.Deleted"] = "The customer feedback has been deleted successfully.",
            ["Admin.Customers.CustomerFeedback.DeleteSelected"] = "Delete selected",
            ["Admin.Customers.CustomerFeedback.Fields.CreatedOn"] = "Created on",
            ["Admin.Customers.CustomerFeedback.Fields.Customer"] = "Customer",
            ["Admin.Customers.CustomerFeedback.Fields.Email"] = "Email",
            ["Admin.Customers.CustomerFeedback.Fields.FeedbackText"] = "Feedback",
            ["Admin.Customers.CustomerFeedback.Fields.FullName"] = "Name",
            ["Admin.Customers.CustomerFeedback.Fields.IsRead"] = "Read",
            ["Admin.Customers.CustomerFeedback.Fields.Rating"] = "Rating",
            ["Admin.Customers.CustomerFeedback.Fields.Store"] = "Store",
            ["Admin.Customers.CustomerFeedback.Fields.Subject"] = "Category",
            ["Admin.Customers.CustomerFeedback.List.CreatedOnFrom"] = "Created from",
            ["Admin.Customers.CustomerFeedback.List.CreatedOnFrom.Hint"] = "The creation from date for the search.",
            ["Admin.Customers.CustomerFeedback.List.CreatedOnTo"] = "Created to",
            ["Admin.Customers.CustomerFeedback.List.CreatedOnTo.Hint"] = "The creation to date for the search.",
            ["Admin.Customers.CustomerFeedback.List.SearchIsRead"] = "Read status",
            ["Admin.Customers.CustomerFeedback.List.SearchIsRead.Hint"] = "Search by read status.",
            ["Admin.Customers.CustomerFeedback.List.SearchSubject"] = "Category",
            ["Admin.Customers.CustomerFeedback.List.SearchSubject.Hint"] = "Search by feedback category.",
            ["Admin.Customers.CustomerFeedback.List.SearchText"] = "Search text",
            ["Admin.Customers.CustomerFeedback.List.SearchText.Hint"] = "Search by name, email, or feedback text.",
            ["Admin.Customers.CustomerFeedback.MarkAsRead"] = "Mark as read",
            ["Admin.Customers.CustomerFeedback.MarkedAsRead"] = "The customer feedback has been marked as read.",
            ["Admin.Customers.CustomerFeedback.ViewDetails"] = "View customer feedback",
            ["Feedback"] = "Feedback",
            ["Feedback.Button"] = "Submit feedback",
            ["Feedback.Category"] = "Category",
            ["Feedback.Category.Bug"] = "Bug",
            ["Feedback.Category.FeatureRequest"] = "Feature request",
            ["Feedback.Category.General"] = "General",
            ["Feedback.Category.Other"] = "Other",
            ["Feedback.Category.Required"] = "Please select a category",
            ["Feedback.Email"] = "Your email",
            ["Feedback.Email.Hint"] = "Enter your email address.",
            ["Feedback.Email.Required"] = "Enter email",
            ["Feedback.FeedbackText"] = "Your feedback",
            ["Feedback.FeedbackText.Hint"] = "Tell us what you think.",
            ["Feedback.FeedbackText.Required"] = "Enter your feedback",
            ["Feedback.FullName"] = "Your name",
            ["Feedback.FullName.Hint"] = "Enter your name.",
            ["Feedback.FullName.Required"] = "Enter your name",
            ["Feedback.Rating"] = "Rating",
            ["Feedback.Rating.Required"] = "Please select a rating",
            ["Feedback.SuccessfullySubmitted"] = "Thank you. Your feedback has been submitted.",
            ["Literals.Nop.Core.Http.NopRouteNames.General.Feedback"] = "Feedback",
            ["PageTitle.Feedback"] = "Submit Feedback"
        };

        var languages = _dataProvider.GetTable<Language>().ToList();
        var existingResources = _dataProvider.GetTable<LocaleStringResource>().ToList();

        foreach (var language in languages)
        {
            foreach (var resource in resources)
            {
                if (existingResources.Any(lsr => lsr.LanguageId == language.Id &&
                    string.Compare(lsr.ResourceName, resource.Key, StringComparison.InvariantCultureIgnoreCase) == 0))
                    continue;

                _dataProvider.InsertEntity(new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = resource.Key,
                    ResourceValue = resource.Value
                });
            }
        }
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
    }
}
