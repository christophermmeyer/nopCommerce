using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-08-25 00:00:00", "Agent interaction window")]
public class AgentInteractionMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<AgentConversationMessage>();
    }
}
