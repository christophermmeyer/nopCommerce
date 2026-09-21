using AwesomeAssertions;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Payments.Manual;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class ManualPaymentProcessorTests
{
    [Test]
    public async Task HidePaymentMethodAsyncAlwaysHidesPublicCardEntry()
    {
        var processor = new ManualPaymentProcessor(
            Mock.Of<IEncryptionService>(),
            Mock.Of<IGenericAttributeService>(),
            Mock.Of<ILocalizationService>(),
            Mock.Of<IOrderTotalCalculationService>(),
            Mock.Of<ISettingService>(),
            Mock.Of<IWebHelper>(),
            Mock.Of<IWorkContext>(),
            new ManualPaymentSettings(),
            new WidgetSettings());

        var hide = await processor.HidePaymentMethodAsync([]);

        hide.Should().BeTrue();
    }
}
