using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents an agent conversation message entity builder
/// </summary>
public partial class AgentConversationMessageBuilder : NopEntityBuilder<AgentConversationMessage>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AgentConversationMessage.CustomerId)).AsInt32().ForeignKey<Customer>()
            .WithColumn(nameof(AgentConversationMessage.StoreId)).AsInt32().ForeignKey<Store>()
            .WithColumn(nameof(AgentConversationMessage.MessageText)).AsString(int.MaxValue).NotNullable();
    }

    #endregion
}
