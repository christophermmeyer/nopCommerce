using AwesomeAssertions;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class PublicStorefrontPaymentDefaultsTests : ServiceTest
{
    [Test]
    public void ActivePaymentMethodsAreCheckMoneyOrderOnly()
    {
        PublicStorefrontPaymentDefaults.ActivePaymentMethodSystemNames.Should()
            .Equal(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);
    }

    [Test]
    public void ForPublicStorefrontRemovesManualAndPayPalAndKeepsCheckMoneyOrder()
    {
        var filtered = PublicStorefrontPaymentDefaults.ForPublicStorefront(
        [
            PublicStorefrontPaymentDefaults.ManualSystemName,
            PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName,
            PublicStorefrontPaymentDefaults.PayPalCommerceSystemName
        ]);

        filtered.Should().Equal(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);
    }

    [Test]
    public void ForPublicStorefrontAddsCheckMoneyOrderWhenMissing()
    {
        var filtered = PublicStorefrontPaymentDefaults.ForPublicStorefront(
        [
            PublicStorefrontPaymentDefaults.ManualSystemName,
            PublicStorefrontPaymentDefaults.PayPalCommerceSystemName
        ]);

        filtered.Should().Equal(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);
    }

    [Test]
    public void ForPublicStorefrontPreservesOtherMethods()
    {
        var filtered = PublicStorefrontPaymentDefaults.ForPublicStorefront(
        [
            PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName,
            "Payments.CustomOffline"
        ]);

        filtered.Should().Equal(
            PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName,
            "Payments.CustomOffline");
    }

    [Test]
    public void ForPublicStorefrontIsCaseInsensitive()
    {
        var filtered = PublicStorefrontPaymentDefaults.ForPublicStorefront(
        [
            "payments.manual",
            "PAYMENTS.PAYPALCOMMERCE",
            "Payments.CheckMoneyOrder"
        ]);

        filtered.Should().Equal(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);
    }

    [Test]
    public void ForPublicStorefrontHandlesNullAndEmpty()
    {
        PublicStorefrontPaymentDefaults.ForPublicStorefront(null)
            .Should().Equal(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);

        PublicStorefrontPaymentDefaults.ForPublicStorefront([])
            .Should().Equal(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);
    }

    [Test]
    public async Task InstalledPaymentSettingsMatchPublicStorefrontPolicy()
    {
        var settingService = GetService<ISettingService>();
        var paymentSettings = await settingService.LoadSettingAsync<PaymentSettings>();

        paymentSettings.ActivePaymentMethodSystemNames.Should()
            .Contain(PublicStorefrontPaymentDefaults.CheckMoneyOrderSystemName);
        paymentSettings.ActivePaymentMethodSystemNames.Should()
            .NotContain(PublicStorefrontPaymentDefaults.ManualSystemName);
        paymentSettings.ActivePaymentMethodSystemNames.Should()
            .NotContain(PublicStorefrontPaymentDefaults.PayPalCommerceSystemName);
        paymentSettings.BypassPaymentMethodSelectionIfOnlyOne.Should().BeTrue();
    }
}
