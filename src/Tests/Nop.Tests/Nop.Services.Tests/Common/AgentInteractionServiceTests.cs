using AwesomeAssertions;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Stores;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class AgentInteractionServiceTests : ServiceTest
{
    private IAgentInteractionService _agentInteractionService;
    private ICustomerService _customerService;
    private IRepository<AgentConversationMessage> _messageRepository;
    private IStoreService _storeService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _agentInteractionService = GetService<IAgentInteractionService>();
        _customerService = GetService<ICustomerService>();
        _messageRepository = GetService<IRepository<AgentConversationMessage>>();
        _storeService = GetService<IStoreService>();
    }

    [Test]
    [TestCase("hello", AgentIntent.Greeting)]
    [TestCase("Hi there", AgentIntent.Greeting)]
    [TestCase("good morning", AgentIntent.Greeting)]
    [TestCase("Where is my order?", AgentIntent.OrderStatus)]
    [TestCase("I need tracking for my orders", AgentIntent.OrderStatus)]
    [TestCase("What are your shipping options?", AgentIntent.Shipping)]
    [TestCase("Can I return this?", AgentIntent.Returns)]
    [TestCase("I want a refund", AgentIntent.Returns)]
    [TestCase("What payment methods do you take?", AgentIntent.Payment)]
    [TestCase("I forgot my password", AgentIntent.Account)]
    [TestCase("I'm looking for a laptop", AgentIntent.ProductSearch)]
    [TestCase("How do I contact support?", AgentIntent.Contact)]
    [TestCase("help", AgentIntent.Contact)]
    [TestCase("asdf qwer", AgentIntent.Unknown)]
    [TestCase("", AgentIntent.Unknown)]
    [TestCase("   ", AgentIntent.Unknown)]
    public void ClassifyIntentShouldMatchExpectedIntent(string message, AgentIntent expected)
    {
        AgentInteractionService.ClassifyIntent(message).Should().Be(expected);
    }

    [Test]
    public void ClassifyIntentShouldNotTreatShippingAsGreeting()
    {
        AgentInteractionService.ClassifyIntent("Tell me about shipping").Should().Be(AgentIntent.Shipping);
    }

    [Test]
    public async Task SendMessageShouldRejectEmptyMessage()
    {
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var store = (await _storeService.GetAllStoresAsync()).First();

        var result = await _agentInteractionService.SendMessageAsync(customer.Id, store.Id, "   ", store.Name);

        result.Success.Should().BeFalse();
        result.ErrorResourceKey.Should().Be("AgentInteraction.Error.MessageRequired");
    }

    [Test]
    public async Task SendMessageShouldRejectMessageThatIsTooLong()
    {
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var store = (await _storeService.GetAllStoresAsync()).First();
        var message = new string('a', NopCommonDefaults.AgentInteractionMaxMessageLength + 1);

        var result = await _agentInteractionService.SendMessageAsync(customer.Id, store.Id, message, store.Name);

        result.Success.Should().BeFalse();
        result.ErrorResourceKey.Should().Be("AgentInteraction.Error.MessageTooLong");
    }

    [Test]
    public async Task SendMessageShouldStoreCustomerMessageAndGreetingReply()
    {
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var store = (await _storeService.GetAllStoresAsync()).First();
        var message = $"hello from agent interaction test {Guid.NewGuid():N}";

        var result = await _agentInteractionService.SendMessageAsync(customer.Id, store.Id, message, store.Name);

        result.Success.Should().BeTrue();
        result.CustomerMessage.Should().NotBeNull();
        result.CustomerMessage.IsFromCustomer.Should().BeTrue();
        result.CustomerMessage.MessageText.Should().Be(message);
        result.AgentMessage.Should().NotBeNull();
        result.AgentMessage.IsFromCustomer.Should().BeFalse();
        result.AgentMessage.MessageText.Should().Contain(store.Name);

        var history = await _agentInteractionService.GetConversationMessagesAsync(customer.Id, store.Id);
        history.Should().Contain(stored => stored.Id == result.CustomerMessage.Id);
        history.Should().Contain(stored => stored.Id == result.AgentMessage.Id);

        await _messageRepository.DeleteAsync(result.CustomerMessage);
        await _messageRepository.DeleteAsync(result.AgentMessage);
    }

    [Test]
    public async Task SendMessageShouldStripHtmlFromCustomerMessage()
    {
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var store = (await _storeService.GetAllStoresAsync()).First();

        var result = await _agentInteractionService.SendMessageAsync(customer.Id, store.Id, "<b>hello</b>", store.Name);

        result.Success.Should().BeTrue();
        result.CustomerMessage.MessageText.Should().Be("hello");

        await _messageRepository.DeleteAsync(result.CustomerMessage);
        await _messageRepository.DeleteAsync(result.AgentMessage);
    }
}
