using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopMigration("2026-09-02 18:20:00", "Admin sidebar navigation scheme localizations", MigrationProcessType.Update)]
public class AdminSidebarSchemeLocalizationMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Admin.Menu.Sidebar.Sections"] = "Admin sections"
        });
    }

    public override void Down()
    {
    }
}
