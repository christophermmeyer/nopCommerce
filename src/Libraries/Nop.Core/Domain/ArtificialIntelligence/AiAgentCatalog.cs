namespace Nop.Core.Domain.ArtificialIntelligence;

/// <summary>
/// Stable keys for the prototype agent catalog. Display text lives in locale resources.
/// </summary>
public static partial class AiAgentCatalog
{
    public const string MaskedSecret = "••••••••••••••••";

    public static class Roles
    {
        public const string ProductDescriptions = "ProductDescriptions";
        public const string CustomerServiceDrafts = "CustomerServiceDrafts";
        public const string MetaTitlesAndDescriptions = "MetaTitlesAndDescriptions";
        public const string CategorySummaries = "CategorySummaries";
        public const string Custom = "Custom";

        public static readonly IReadOnlyList<string> All = new[]
        {
            ProductDescriptions,
            CustomerServiceDrafts,
            MetaTitlesAndDescriptions,
            CategorySummaries,
            Custom
        };
    }

    public static class Providers
    {
        public const string OpenAiCompatible = "OpenAiCompatible";
        public const string Anthropic = "Anthropic";
        public const string AzureOpenAi = "AzureOpenAi";
        public const string CustomEndpoint = "CustomEndpoint";

        public static readonly IReadOnlyList<string> All = new[]
        {
            OpenAiCompatible,
            Anthropic,
            AzureOpenAi,
            CustomEndpoint
        };
    }

    public static class Permissions
    {
        public const string ReadProductCatalog = "ReadProductCatalog";
        public const string DraftProductDescriptions = "DraftProductDescriptions";
        public const string DraftCategoryCopy = "DraftCategoryCopy";
        public const string SuggestSeoMeta = "SuggestSeoMeta";
        public const string ReadCustomerMessages = "ReadCustomerMessages";
        public const string DraftSupportReplies = "DraftSupportReplies";
        public const string ReadOrderSummaries = "ReadOrderSummaries";

        public const string ReadCustomerPii = "ReadCustomerPii";
        public const string ReadPaymentDetails = "ReadPaymentDetails";
        public const string PublishWithoutReview = "PublishWithoutReview";

        public static readonly IReadOnlyList<string> Grantable = new[]
        {
            ReadProductCatalog,
            DraftProductDescriptions,
            DraftCategoryCopy,
            SuggestSeoMeta,
            ReadCustomerMessages,
            DraftSupportReplies,
            ReadOrderSummaries
        };

        public static readonly IReadOnlyList<string> AlwaysDenied = new[]
        {
            ReadCustomerPii,
            ReadPaymentDetails,
            PublishWithoutReview
        };
    }
}
