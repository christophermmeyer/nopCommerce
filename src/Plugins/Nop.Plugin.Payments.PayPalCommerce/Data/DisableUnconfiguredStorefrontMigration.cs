using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Payments.PayPalCommerce.Data;

/// <summary>
/// MEYER-16: hide PayPal buttons on PDP/cart until merchant credentials exist.
/// Do not invent sandbox credentials.
/// </summary>
[NopMigration("2026-09-21 14:00:00", "Payments.PayPalCommerce 5.00.6. Disable unconfigured public buttons", MigrationProcessType.Update)]
public class DisableUnconfiguredStorefrontMigration : MigrationBase
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.SetSetting<PayPalCommerceSettings, bool>(settings => settings.DisplayButtonsOnProductDetails,
            setting => setting.DisplayButtonsOnProductDetails = false);
        this.SetSetting<PayPalCommerceSettings, bool>(settings => settings.DisplayButtonsOnShoppingCart,
            setting => setting.DisplayButtonsOnShoppingCart = false);
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }
}
