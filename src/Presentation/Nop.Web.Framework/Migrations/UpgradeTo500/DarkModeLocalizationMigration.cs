using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-08-14 16:00:00", "5.00", UpdateMigrationType.Localization)]
public class DarkModeLocalizationMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["DarkMode.Toggle"] = "Dark mode",
            ["DarkMode.Toggle.Enable"] = "Switch to dark mode",
            ["DarkMode.Toggle.Disable"] = "Switch to light mode"
        });
    }

    public override void Down()
    {
    }
}
