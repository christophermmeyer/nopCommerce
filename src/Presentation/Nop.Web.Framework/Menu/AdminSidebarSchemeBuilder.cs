namespace Nop.Web.Framework.Menu;

/// <summary>
/// Builds the icon-rail / section-panel admin navigation model
/// </summary>
public static class AdminSidebarSchemeBuilder
{
    /// <summary>
    /// Maps a visible admin menu tree onto rail sections
    /// </summary>
    /// <param name="root">Root menu item whose children become rail sections</param>
    /// <param name="activeSystemName">System name of the current page</param>
    /// <returns>Sidebar scheme model</returns>
    public static AdminSidebarSchemeModel Build(AdminMenuItem root, string activeSystemName)
    {
        ArgumentNullException.ThrowIfNull(root);

        var scheme = new AdminSidebarSchemeModel();

        foreach (var node in root.ChildNodes.Where(n => n.Visible))
            scheme.Sections.Add(MapSection(node, activeSystemName));

        if (scheme.Sections.Count > 0 && !scheme.Sections.Any(section => section.IsActive))
        {
            var fallback = scheme.Sections.FirstOrDefault(section => section.Items.Count > 0)
                ?? scheme.Sections[0];
            fallback.IsActive = true;
        }

        return scheme;
    }

    private static AdminSidebarSectionModel MapSection(AdminMenuItem node, string activeSystemName)
    {
        var section = new AdminSidebarSectionModel
        {
            SystemName = node.SystemName,
            Title = node.Title,
            IconClass = string.IsNullOrEmpty(node.IconClass) ? "fas fa-circle" : node.IconClass,
            Url = node.Url,
            OpenUrlInNewTab = node.OpenUrlInNewTab,
            IsActive = ContainsSystemName(node, activeSystemName),
            HtmlId = ToHtmlId(node.SystemName)
        };

        foreach (var child in node.ChildNodes.Where(n => n.Visible))
            section.Items.Add(MapItem(child, activeSystemName, node.Title, null));

        return section;
    }

    private static AdminSidebarItemModel MapItem(AdminMenuItem node, string activeSystemName, string parentTitle, string grandParentTitle)
    {
        var item = new AdminSidebarItemModel
        {
            SystemName = node.SystemName,
            Title = node.Title,
            IconClass = node.IconClass,
            Url = node.Url,
            OpenUrlInNewTab = node.OpenUrlInNewTab,
            ParentTitle = parentTitle,
            GrandParentTitle = grandParentTitle,
            IsActive = ContainsSystemName(node, activeSystemName)
        };

        foreach (var child in node.ChildNodes.Where(n => n.Visible))
            item.Items.Add(MapItem(child, activeSystemName, node.Title, parentTitle));

        item.IsExpanded = item.IsActive;

        return item;
    }

    private static bool ContainsSystemName(AdminMenuItem item, string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return false;

        if (string.Equals(item.SystemName, systemName, StringComparison.Ordinal))
            return true;

        return item.ChildNodes.Any(child => ContainsSystemName(child, systemName));
    }

    private static string ToHtmlId(string systemName)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            return "section";

        var chars = systemName.Select(ch => char.IsLetterOrDigit(ch) ? char.ToLowerInvariant(ch) : '-').ToArray();
        var id = new string(chars).Trim('-');

        return string.IsNullOrEmpty(id) ? "section" : id;
    }
}
