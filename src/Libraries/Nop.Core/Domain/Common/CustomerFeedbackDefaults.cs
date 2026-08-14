namespace Nop.Core.Domain.Common;

/// <summary>
/// Represents default values related to customer feedback
/// </summary>
public static partial class CustomerFeedbackDefaults
{
    public const string CategoryGeneral = "General";
    public const string CategoryBug = "Bug";
    public const string CategoryFeatureRequest = "FeatureRequest";
    public const string CategoryOther = "Other";

    public static readonly string[] Categories =
    [
        CategoryGeneral,
        CategoryBug,
        CategoryFeatureRequest,
        CategoryOther
    ];
}
