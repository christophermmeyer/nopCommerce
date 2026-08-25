using System.Text.RegularExpressions;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Common;

/// <summary>
/// Agent interaction service
/// </summary>
public partial class AgentInteractionService : IAgentInteractionService
{
    #region Fields

    protected readonly ArtificialIntelligenceHttpClient _artificialIntelligenceHttpClient;
    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IRepository<AgentConversationMessage> _messageRepository;

    #endregion

    #region Ctor

    public AgentInteractionService(ArtificialIntelligenceHttpClient artificialIntelligenceHttpClient,
        ArtificialIntelligenceSettings artificialIntelligenceSettings,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        ILogger logger,
        IRepository<AgentConversationMessage> messageRepository)
    {
        _artificialIntelligenceHttpClient = artificialIntelligenceHttpClient;
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _logger = logger;
        _messageRepository = messageRepository;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Tokenizes a customer message for intent matching
    /// </summary>
    protected static IList<string> Tokenize(string message)
    {
        return Regex.Split(message.Trim().ToLowerInvariant(), @"[^\p{L}\p{N}]+", RegexOptions.CultureInvariant)
            .Where(token => token.Length > 0)
            .ToList();
    }

    /// <summary>
    /// Indicates whether any token matches a known keyword
    /// </summary>
    protected static bool ContainsAny(IList<string> tokens, params string[] keywords)
    {
        return tokens.Any(token => keywords.Contains(token, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Indicates whether the configured AI provider has an API key
    /// </summary>
    protected virtual bool CanUseArtificialIntelligence()
    {
        if (!_artificialIntelligenceSettings.Enabled)
            return false;

        return _artificialIntelligenceSettings.ProviderType switch
        {
            ArtificialIntelligenceProviderType.Gemini => !string.IsNullOrWhiteSpace(_artificialIntelligenceSettings.GeminiApiKey),
            ArtificialIntelligenceProviderType.ChatGpt => !string.IsNullOrWhiteSpace(_artificialIntelligenceSettings.ChatGptApiKey),
            ArtificialIntelligenceProviderType.DeepSeek => !string.IsNullOrWhiteSpace(_artificialIntelligenceSettings.DeepSeekApiKey),
            _ => false
        };
    }

    /// <summary>
    /// Builds a store-assistant prompt for the configured AI provider
    /// </summary>
    protected virtual string BuildArtificialIntelligenceQuery(string message, string storeName)
    {
        return $"""
            You are a helpful shopping assistant for the online store "{storeName}".
            Answer the customer briefly in plain text. Do not invent order details, prices, or policies you were not given.
            If you are unsure, tell the customer how to use search, their account orders page, or the contact us form.
            Customer message: {message}
            """;
    }

    /// <summary>
    /// Gets a localized fallback reply for a classified intent
    /// </summary>
    protected virtual async Task<string> GetFallbackReplyAsync(AgentIntent intent, string storeName)
    {
        var resourceKey = intent switch
        {
            AgentIntent.Greeting => "AgentInteraction.Reply.Greeting",
            AgentIntent.OrderStatus => "AgentInteraction.Reply.OrderStatus",
            AgentIntent.Shipping => "AgentInteraction.Reply.Shipping",
            AgentIntent.Returns => "AgentInteraction.Reply.Returns",
            AgentIntent.Payment => "AgentInteraction.Reply.Payment",
            AgentIntent.Account => "AgentInteraction.Reply.Account",
            AgentIntent.ProductSearch => "AgentInteraction.Reply.ProductSearch",
            AgentIntent.Contact => "AgentInteraction.Reply.Contact",
            _ => "AgentInteraction.Reply.Unknown"
        };

        var reply = await _localizationService.GetResourceAsync(resourceKey);
        return string.Format(reply, storeName);
    }

    /// <summary>
    /// Generates an agent reply, using AI when configured and falling back otherwise
    /// </summary>
    protected virtual async Task<string> GenerateReplyAsync(string message, string storeName)
    {
        if (CanUseArtificialIntelligence())
        {
            try
            {
                var aiReply = await _artificialIntelligenceHttpClient.SendQueryAsync(BuildArtificialIntelligenceQuery(message, storeName));
                if (!string.IsNullOrWhiteSpace(aiReply))
                    return aiReply.Trim();
            }
            catch (Exception exception)
            {
                await _logger.InsertLogAsync(LogLevel.Error, "Storefront agent interaction AI request failed", exception.ToString());
            }
        }

        return await GetFallbackReplyAsync(ClassifyIntent(message), storeName);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Classifies a customer message into a storefront agent intent
    /// </summary>
    /// <param name="message">Customer message text</param>
    /// <returns>Classified intent</returns>
    public static AgentIntent ClassifyIntent(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return AgentIntent.Unknown;

        var normalized = message.Trim().ToLowerInvariant();
        var tokens = Tokenize(normalized);

        if (normalized is "hi" or "hello" or "hey" or "howdy"
            || ContainsAny(tokens, "hello", "hey", "howdy")
            || (tokens.Count <= 4 && ContainsAny(tokens, "hi"))
            || normalized.Contains("good morning", StringComparison.Ordinal)
            || normalized.Contains("good afternoon", StringComparison.Ordinal)
            || normalized.Contains("good evening", StringComparison.Ordinal))
            return AgentIntent.Greeting;

        if (ContainsAny(tokens, "order", "orders", "tracking", "track", "tracked")
            || normalized.Contains("order status", StringComparison.Ordinal)
            || normalized.Contains("where is my", StringComparison.Ordinal))
            return AgentIntent.OrderStatus;

        if (ContainsAny(tokens, "shipping", "shipment", "delivery", "deliver", "postage", "ship"))
            return AgentIntent.Shipping;

        if (ContainsAny(tokens, "return", "returns", "refund", "refunds", "exchange", "exchanges"))
            return AgentIntent.Returns;

        if (ContainsAny(tokens, "payment", "payments", "pay", "checkout", "invoice")
            || normalized.Contains("credit card", StringComparison.Ordinal)
            || normalized.Contains("debit card", StringComparison.Ordinal))
            return AgentIntent.Payment;

        if (ContainsAny(tokens, "account", "login", "password", "register", "registration", "signin")
            || normalized.Contains("sign in", StringComparison.Ordinal)
            || normalized.Contains("sign up", StringComparison.Ordinal)
            || normalized.Contains("log in", StringComparison.Ordinal))
            return AgentIntent.Account;

        if (ContainsAny(tokens, "product", "products", "buy", "search", "find", "recommend", "recommendation", "looking")
            || normalized.Contains("looking for", StringComparison.Ordinal))
            return AgentIntent.ProductSearch;

        if (ContainsAny(tokens, "contact", "support", "email", "phone", "human", "agent", "helpdesk")
            || normalized is "help")
            return AgentIntent.Contact;

        return AgentIntent.Unknown;
    }

    /// <summary>
    /// Gets recent conversation messages for a customer in a store
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the conversation messages
    /// </returns>
    public virtual async Task<IList<AgentConversationMessage>> GetConversationMessagesAsync(int customerId, int storeId)
    {
        var messages = await _messageRepository.Table
            .Where(message => message.CustomerId == customerId && message.StoreId == storeId)
            .OrderByDescending(message => message.CreatedOnUtc)
            .ThenByDescending(message => message.Id)
            .Take(NopCommonDefaults.AgentInteractionHistorySize)
            .ToListAsync();

        return messages
            .OrderBy(message => message.CreatedOnUtc)
            .ThenBy(message => message.Id)
            .ToList();
    }

    /// <summary>
    /// Sends a customer message and returns the stored customer message plus the agent reply
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="message">Customer message text</param>
    /// <param name="storeName">Current store name used in replies</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the send result
    /// </returns>
    public virtual async Task<AgentInteractionSendResult> SendMessageAsync(int customerId, int storeId, string message, string storeName)
    {
        var sanitized = _htmlFormatter.StripTags(message ?? string.Empty)?.Trim();
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return new AgentInteractionSendResult
            {
                Success = false,
                ErrorResourceKey = "AgentInteraction.Error.MessageRequired"
            };
        }

        if (sanitized.Length > NopCommonDefaults.AgentInteractionMaxMessageLength)
        {
            return new AgentInteractionSendResult
            {
                Success = false,
                ErrorResourceKey = "AgentInteraction.Error.MessageTooLong"
            };
        }

        var now = DateTime.UtcNow;
        var customerMessage = new AgentConversationMessage
        {
            CustomerId = customerId,
            StoreId = storeId,
            IsFromCustomer = true,
            MessageText = sanitized,
            CreatedOnUtc = now
        };

        await _messageRepository.InsertAsync(customerMessage);

        var replyText = await GenerateReplyAsync(sanitized, storeName);
        var agentMessage = new AgentConversationMessage
        {
            CustomerId = customerId,
            StoreId = storeId,
            IsFromCustomer = false,
            MessageText = replyText,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _messageRepository.InsertAsync(agentMessage);

        return new AgentInteractionSendResult
        {
            Success = true,
            CustomerMessage = customerMessage,
            AgentMessage = agentMessage
        };
    }

    #endregion
}
