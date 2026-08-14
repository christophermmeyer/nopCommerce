using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a customer feedback entity builder
/// </summary>
public partial class CustomerFeedbackBuilder : NopEntityBuilder<CustomerFeedback>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerFeedback.CustomerId)).AsInt32().ForeignKey<Customer>()
            .WithColumn(nameof(CustomerFeedback.StoreId)).AsInt32().ForeignKey<Store>()
            .WithColumn(nameof(CustomerFeedback.FullName)).AsString(400).NotNullable()
            .WithColumn(nameof(CustomerFeedback.Email)).AsString(400).NotNullable()
            .WithColumn(nameof(CustomerFeedback.Subject)).AsString(400).NotNullable()
            .WithColumn(nameof(CustomerFeedback.FeedbackText)).AsString(int.MaxValue).NotNullable();
    }

    #endregion
}
