using AwesomeAssertions;
using Nop.Web.Framework.Menu;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Menu;

[TestFixture]
public class AdminSidebarSchemeBuilderTests
{
    [Test]
    public void Build_ThrowsWhenRootIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => AdminSidebarSchemeBuilder.Build(null, "Dashboard"));
    }

    [Test]
    public void Build_MapsVisibleSectionsAndSkipsHiddenOnes()
    {
        var scheme = AdminSidebarSchemeBuilder.Build(CreateMenu(), null);

        scheme.Sections.Select(section => section.SystemName).Should().Equal("Dashboard", "Catalog");
        scheme.Sections.Should().NotContain(section => section.SystemName == "Hidden");
    }

    [Test]
    public void Build_TreatsUrlOnlyRootItemAsLeaf()
    {
        var scheme = AdminSidebarSchemeBuilder.Build(CreateMenu(), "Dashboard");
        var dashboard = scheme.Sections.Single(section => section.SystemName == "Dashboard");

        dashboard.IsLeaf.Should().BeTrue();
        dashboard.Url.Should().Be("/admin");
        dashboard.Items.Should().BeEmpty();
        dashboard.IsActive.Should().BeTrue();
    }

    [Test]
    public void Build_MarksSectionAndNestedGroupThatContainTheActivePage()
    {
        var scheme = AdminSidebarSchemeBuilder.Build(CreateMenu(), "Product attributes");
        var catalog = scheme.Sections.Single(section => section.SystemName == "Catalog");
        var attributes = catalog.Items.Single(item => item.SystemName == "Attributes");

        catalog.IsActive.Should().BeTrue();
        catalog.IsLeaf.Should().BeFalse();
        attributes.IsGroup.Should().BeTrue();
        attributes.IsActive.Should().BeTrue();
        attributes.IsExpanded.Should().BeTrue();
        attributes.Items.Single(item => item.SystemName == "Product attributes").IsActive.Should().BeTrue();
        attributes.Items.Single(item => item.SystemName == "Checkout attributes").IsActive.Should().BeFalse();
    }

    [Test]
    public void Build_SetsSearchBreadcrumbsOnNestedLinks()
    {
        var scheme = AdminSidebarSchemeBuilder.Build(CreateMenu(), "Product attributes");
        var productAttributes = scheme.Sections
            .SelectMany(section => section.Items)
            .SelectMany(item => item.Items)
            .Single(item => item.SystemName == "Product attributes");

        productAttributes.ParentTitle.Should().Be("Attributes");
        productAttributes.GrandParentTitle.Should().Be("Catalog");
    }

    [Test]
    public void Build_FallsBackToFirstSectionWithItemsWhenNothingIsActive()
    {
        var scheme = AdminSidebarSchemeBuilder.Build(CreateMenu(), "missing");

        scheme.Sections.Single(section => section.SystemName == "Catalog").IsActive.Should().BeTrue();
        scheme.Sections.Single(section => section.SystemName == "Dashboard").IsActive.Should().BeFalse();
    }

    [Test]
    public void Build_UsesSafeHtmlIdsAndDefaultIcon()
    {
        var root = new AdminMenuItem
        {
            ChildNodes =
            [
                new AdminMenuItem
                {
                    SystemName = "Content Management",
                    Title = "Content",
                    Visible = true,
                    ChildNodes =
                    [
                        new AdminMenuItem { SystemName = "Topics", Title = "Topics", Url = "/topics", Visible = true }
                    ]
                },
                new AdminMenuItem
                {
                    SystemName = "Other",
                    Title = "Other",
                    Url = "/other",
                    Visible = true
                }
            ]
        };

        var scheme = AdminSidebarSchemeBuilder.Build(root, "Topics");
        var content = scheme.Sections.Single(section => section.SystemName == "Content Management");
        var other = scheme.Sections.Single(section => section.SystemName == "Other");

        content.HtmlId.Should().Be("content-management");
        content.IconClass.Should().Be("fas fa-circle");
        other.HtmlId.Should().Be("other");
    }

    [Test]
    public void Build_SkipsInvisibleChildrenInsideAVisibleSection()
    {
        var scheme = AdminSidebarSchemeBuilder.Build(CreateMenu(), "Products");
        var catalog = scheme.Sections.Single(section => section.SystemName == "Catalog");

        catalog.Items.Select(item => item.SystemName).Should().Equal("Products", "Attributes");
    }

    private static AdminMenuItem CreateMenu()
    {
        return new AdminMenuItem
        {
            SystemName = "Home",
            ChildNodes =
            [
                new AdminMenuItem
                {
                    SystemName = "Dashboard",
                    Title = "Dashboard",
                    Url = "/admin",
                    IconClass = "fas fa-desktop",
                    Visible = true
                },
                new AdminMenuItem
                {
                    SystemName = "Catalog",
                    Title = "Catalog",
                    IconClass = "fas fa-book",
                    Visible = true,
                    ChildNodes =
                    [
                        new AdminMenuItem
                        {
                            SystemName = "Products",
                            Title = "Products",
                            Url = "/products",
                            Visible = true
                        },
                        new AdminMenuItem
                        {
                            SystemName = "Hidden product",
                            Title = "Hidden product",
                            Url = "/hidden-product",
                            Visible = false
                        },
                        new AdminMenuItem
                        {
                            SystemName = "Attributes",
                            Title = "Attributes",
                            Visible = true,
                            ChildNodes =
                            [
                                new AdminMenuItem
                                {
                                    SystemName = "Product attributes",
                                    Title = "Product attributes",
                                    Url = "/product-attributes",
                                    Visible = true
                                },
                                new AdminMenuItem
                                {
                                    SystemName = "Checkout attributes",
                                    Title = "Checkout attributes",
                                    Url = "/checkout-attributes",
                                    Visible = true
                                }
                            ]
                        }
                    ]
                },
                new AdminMenuItem
                {
                    SystemName = "Hidden",
                    Title = "Hidden",
                    Url = "/hidden",
                    Visible = false
                }
            ]
        };
    }
}
