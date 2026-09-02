var Admin = Admin || {};

Admin.SidebarScheme = (function () {
    var storageKey = "nop.admin.sidebarSection";

    var selectSection = function (systemName, persist) {
        if (!systemName)
            return;

        var $items = $(".admin-nav-rail-item");
        var $target = $items.filter(function () {
            return $(this).attr("data-section") === systemName;
        });

        if (!$target.length)
            return;

        $items.removeClass("active").attr("aria-selected", "false");
        $target.addClass("active");
        if ($target.is("[data-has-panel]"))
            $target.attr("aria-selected", "true");

        var $panels = $(".admin-nav-panel");
        $panels.removeClass("is-active").attr("hidden", true);

        var $panel = $panels.filter(function () {
            return $(this).attr("data-section") === systemName;
        });

        if ($panel.length) {
            $panel.addClass("is-active").removeAttr("hidden");
            $(".admin-nav").removeClass("admin-nav-leaf-only");
        } else {
            $(".admin-nav").addClass("admin-nav-leaf-only");
        }

        if (persist) {
            try {
                localStorage.setItem(storageKey, systemName);
            } catch (e) { }
        }
    };

    var init = function () {
        if (!$("[data-admin-nav]").length)
            return;

        var $active = $(".admin-nav-rail-item.active");
        if ($active.length) {
            try {
                localStorage.setItem(storageKey, $active.attr("data-section"));
            } catch (e) { }
        }

        $(document).on("click", ".admin-nav-rail-item[data-has-panel]", function (e) {
            e.preventDefault();
            selectSection($(this).attr("data-section"), true);
        });

        $(document).on("click", ".admin-nav-group-toggle", function (e) {
            e.preventDefault();
            var $group = $(this).closest(".admin-nav-group");
            var open = !$group.hasClass("is-open");
            $group.toggleClass("is-open", open);
            $(this).attr("aria-expanded", open);
        });
    };

    return {
        init: init,
        selectSection: selectSection
    };
})();
