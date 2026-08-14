using AwesomeAssertions;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class CustomerFeedbackServiceTests : ServiceTest
{
    private ICustomerFeedbackService _customerFeedbackService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _customerFeedbackService = GetService<ICustomerFeedbackService>();
    }

    [Test]
    public async Task CanInsertGetUpdateAndDeleteCustomerFeedback()
    {
        var feedback = new CustomerFeedback
        {
            CustomerId = 1,
            StoreId = 1,
            FullName = "Jane Doe",
            Email = "jane@example.com",
            Subject = CustomerFeedbackDefaults.CategoryGeneral,
            FeedbackText = "The site is easy to use.",
            Rating = 5,
            IsRead = false,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _customerFeedbackService.InsertCustomerFeedbackAsync(feedback);
        feedback.Id.Should().BeGreaterThan(0);

        var loaded = await _customerFeedbackService.GetCustomerFeedbackByIdAsync(feedback.Id);
        loaded.Should().NotBeNull();
        loaded.Email.Should().Be("jane@example.com");
        loaded.Subject.Should().Be(CustomerFeedbackDefaults.CategoryGeneral);
        loaded.Rating.Should().Be(5);
        loaded.IsRead.Should().BeFalse();

        loaded.IsRead = true;
        await _customerFeedbackService.UpdateCustomerFeedbackAsync(loaded);

        var updated = await _customerFeedbackService.GetCustomerFeedbackByIdAsync(feedback.Id);
        updated.IsRead.Should().BeTrue();

        var all = await _customerFeedbackService.GetAllCustomerFeedbackAsync(searchText: "easy to use");
        all.Any(item => item.Id == feedback.Id).Should().BeTrue();

        await _customerFeedbackService.DeleteCustomerFeedbackAsync(updated);
        (await _customerFeedbackService.GetCustomerFeedbackByIdAsync(feedback.Id)).Should().BeNull();
    }
}
