using AwesomeAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public;

[TestFixture]
public class DarkModeTests : BaseNopTest
{
    [Test]
    public void HeaderShouldRenderDarkModeToggle()
    {
        var fileProvider = CommonHelper.DefaultFileProvider;
        var headerPath = fileProvider.MapPath("~/Views/Shared/_Header.cshtml");

        fileProvider.FileExists(headerPath).Should().BeTrue();

        var header = fileProvider.ReadAllText(headerPath, System.Text.Encoding.UTF8);
        header.Should().Contain("data-dark-mode-toggle");
        header.Should().Contain("DarkMode.Toggle");
        header.Should().Contain("dark-mode-selector");
    }

    [Test]
    public void RootHeadShouldApplyStoredColorSchemeBeforeStyles()
    {
        var fileProvider = CommonHelper.DefaultFileProvider;
        var headPath = fileProvider.MapPath("~/Views/Shared/_Root.Head.cshtml");

        fileProvider.FileExists(headPath).Should().BeTrue();

        var head = fileProvider.ReadAllText(headPath, System.Text.Encoding.UTF8);
        head.Should().Contain("nop.colorScheme");
        head.Should().Contain("public.darkmode.js");
        head.Should().Contain("prefers-color-scheme");
        head.Should().Contain("dark-mode");
    }

    [Test]
    public void DefaultCleanHeadShouldLoadDarkModeStylesheetAfterThemeCss()
    {
        var fileProvider = CommonHelper.DefaultFileProvider;
        var themeHeadPath = fileProvider.MapPath("~/Themes/DefaultClean/Views/Shared/Head.cshtml");

        fileProvider.FileExists(themeHeadPath).Should().BeTrue();

        var themeHead = fileProvider.ReadAllText(themeHeadPath, System.Text.Encoding.UTF8);
        themeHead.Should().Contain("AddCssFileParts");
        themeHead.Should().Contain("dark-mode.css");
    }
}
