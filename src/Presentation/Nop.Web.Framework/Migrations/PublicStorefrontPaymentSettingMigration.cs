using FluentMigrator;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Payments;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations;

/// <summary>
/// MEYER-16: deactivate Manual card entry and unconfigured PayPal on the public storefront.
/// Check/Money Order stays available for offline B2B.
/// </summary>
[NopUpdateMigration("2026-09-21 14:00:00", "5.00", UpdateMigrationType.Settings)]
public class PublicStorefrontPaymentSettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.SetSetting<PaymentSettings, List<string>>(settings => settings.ActivePaymentMethodSystemNames, setting =>
        {
            setting.ActivePaymentMethodSystemNames =
                PublicStorefrontPaymentDefaults.ForPublicStorefront(setting.ActivePaymentMethodSystemNames);
        });
    }

    public override void Down()
    {
        //add the downgrade logic if necessary
    }
}
