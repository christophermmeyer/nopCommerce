using FluentValidation.TestHelper;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Common;

[TestFixture]
public class FeedbackValidatorTests : BaseNopTest
{
    private FeedbackValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new FeedbackValidator(GetService<ILocalizationService>());
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
        var model = new FeedbackModel
        {
            Email = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
        var model = new FeedbackModel
        {
            Email = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
        var model = new FeedbackModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldHaveErrorWhenFullNameIsNullOrEmpty()
    {
        var model = new FeedbackModel
        {
            FullName = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
        model.FullName = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenFullNameIsSpecified()
    {
        var model = new FeedbackModel
        {
            FullName = "John Smith"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Test]
    public void ShouldHaveErrorWhenFeedbackTextIsNullOrEmpty()
    {
        var model = new FeedbackModel
        {
            FeedbackText = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FeedbackText);
        model.FeedbackText = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FeedbackText);
    }

    [Test]
    public void ShouldNotHaveErrorWhenFeedbackTextIsSpecified()
    {
        var model = new FeedbackModel
        {
            FeedbackText = "The checkout flow was easy to use."
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FeedbackText);
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsMissingOrUnknown()
    {
        var model = new FeedbackModel
        {
            Subject = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = "NotARealCategory";
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
    }

    [Test]
    public void ShouldNotHaveErrorWhenSubjectIsKnownCategory()
    {
        var model = new FeedbackModel
        {
            Subject = CustomerFeedbackDefaults.CategoryFeatureRequest
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
    }

    [Test]
    public void ShouldHaveErrorWhenRatingIsOutOfRange()
    {
        var model = new FeedbackModel
        {
            Rating = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Rating);
        model.Rating = 6;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Test]
    public void ShouldNotHaveErrorWhenRatingIsValid()
    {
        var model = new FeedbackModel
        {
            Rating = 5
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Rating);
    }
}
