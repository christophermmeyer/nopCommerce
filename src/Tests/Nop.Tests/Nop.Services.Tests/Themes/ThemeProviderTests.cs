using AwesomeAssertions;
using Nop.Core;
using Nop.Services.Themes;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Themes;

[TestFixture]
public class ThemeProviderTests : BaseNopTest
{
    private IThemeProvider _themeProvider;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        _themeProvider = GetService<IThemeProvider>();
    }

    [Test]
    public void CanGetThemeDescriptorFromText()
    {
        var descriptor = "{ \"SystemName\": \"Test system name\", \"FriendlyName\": \"Test\", \"SupportRTL\": false, \"PreviewImageUrl\": \"~/Themes/Test/preview.jpg\", \"PreviewText\": \"The 'Test' site theme\" }";

        var themeDescriptor = _themeProvider.GetThemeDescriptorFromText(descriptor);

        themeDescriptor.Should().NotBeNull();
        themeDescriptor.SystemName.Should().BeEquivalentTo("Test system name");
    }

    [Test]
    public async Task CanGetThemes()
    {
        var themes = await _themeProvider.GetThemesAsync();

        themes.Count.Should().BeGreaterThan(0);
    }


    [Test]
    public async Task CanGetThemeBySystemName()
    {
        var themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("Test system name");
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(string.Empty);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(null);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("DefaultClean");
        themeDescriptor.Should().NotBeNull();
        themeDescriptor.FriendlyName.Should().BeEquivalentTo("Default clean");
    }


    [Test]
    public async Task CanThemeExists()
    {
        var isExists = await _themeProvider.ThemeExistsAsync("Test system name");
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync(string.Empty);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync(null);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync("DefaultClean");
        isExists.Should().BeTrue();
    }

    [Test]
    public void DefaultCleanThemeShouldIncludeDarkModeStylesheet()
    {
        var fileProvider = CommonHelper.DefaultFileProvider;
        var darkModeCss = fileProvider.MapPath("~/Themes/DefaultClean/Content/css/dark-mode.css");

        fileProvider.FileExists(darkModeCss).Should().BeTrue();

        var css = fileProvider.ReadAllText(darkModeCss, System.Text.Encoding.UTF8);
        css.Should().Contain("html.dark-mode");
        css.Should().Contain("color-scheme: dark");
        css.Should().Contain(".dark-mode-toggle");
    }

    [Test]
    public void PublicStoreShouldIncludeDarkModeScript()
    {
        var fileProvider = CommonHelper.DefaultFileProvider;
        var scriptPath = fileProvider.MapPath("~/js/public.darkmode.js");

        fileProvider.FileExists(scriptPath).Should().BeTrue();

        var script = fileProvider.ReadAllText(scriptPath, System.Text.Encoding.UTF8);
        script.Should().Contain("nop.colorScheme");
        script.Should().Contain("data-dark-mode-toggle");
    }

    [Test]
    public void DefaultResourcesShouldIncludeDarkModeLocaleKeys()
    {
        var fileProvider = CommonHelper.DefaultFileProvider;
        var resourcesPath = fileProvider.MapPath("~/App_Data/Localization/defaultResources.nopres.xml");

        fileProvider.FileExists(resourcesPath).Should().BeTrue();

        var resources = fileProvider.ReadAllText(resourcesPath, System.Text.Encoding.UTF8);
        resources.Should().Contain("DarkMode.Toggle");
        resources.Should().Contain("DarkMode.Toggle.Enable");
        resources.Should().Contain("DarkMode.Toggle.Disable");
    }
}