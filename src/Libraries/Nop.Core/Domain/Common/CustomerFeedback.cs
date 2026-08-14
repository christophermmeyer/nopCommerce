namespace Nop.Core.Domain.Common;

/// <summary>
/// Represents customer feedback submitted from the public store
/// </summary>
public partial class CustomerFeedback : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the submitter's full name
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the submitter's email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the feedback category
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the feedback text
    /// </summary>
    public string FeedbackText { get; set; }

    /// <summary>
    /// Gets or sets the rating (1-5)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an administrator has read the feedback
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
