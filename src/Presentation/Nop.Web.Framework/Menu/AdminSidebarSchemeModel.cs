namespace Nop.Web.Framework.Menu;

/// <summary>
/// Two-pane admin sidebar: icon rail plus the pages in the selected section
/// </summary>
public partial class AdminSidebarSchemeModel
{
    /// <summary>
    /// Top-level rail sections
    /// </summary>
    public IList<AdminSidebarSectionModel> Sections { get; set; } = [];
}

/// <summary>
/// A top-level sidebar section (rail icon, optional page list)
/// </summary>
public partial class AdminSidebarSectionModel
{
    /// <summary>
    /// System name
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Display title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Font Awesome icon class
    /// </summary>
    public string IconClass { get; set; }

    /// <summary>
    /// Direct URL when the section is a leaf (no child pages)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Open the leaf URL in a new tab
    /// </summary>
    public bool OpenUrlInNewTab { get; set; }

    /// <summary>
    /// Whether this section contains the current page
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// CSS-safe identifier used for tab/panel pairing
    /// </summary>
    public string HtmlId { get; set; }

    /// <summary>
    /// Child links and groups shown in the section panel
    /// </summary>
    public IList<AdminSidebarItemModel> Items { get; set; } = [];

    /// <summary>
    /// True when the rail item navigates instead of opening a panel
    /// </summary>
    public bool IsLeaf => !string.IsNullOrEmpty(Url) && Items.Count == 0;
}

/// <summary>
/// A page link or nested group inside a sidebar section
/// </summary>
public partial class AdminSidebarItemModel
{
    /// <summary>
    /// System name
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Display title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Font Awesome icon class
    /// </summary>
    public string IconClass { get; set; }

    /// <summary>
    /// Page URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Open the URL in a new tab
    /// </summary>
    public bool OpenUrlInNewTab { get; set; }

    /// <summary>
    /// Immediate parent title (for search breadcrumbs)
    /// </summary>
    public string ParentTitle { get; set; }

    /// <summary>
    /// Grandparent title (for search breadcrumbs)
    /// </summary>
    public string GrandParentTitle { get; set; }

    /// <summary>
    /// Whether this item or a descendant is the current page
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether a group should render expanded
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Nested items
    /// </summary>
    public IList<AdminSidebarItemModel> Items { get; set; } = [];

    /// <summary>
    /// True when this node is a group header
    /// </summary>
    public bool IsGroup => Items.Count > 0;
}
