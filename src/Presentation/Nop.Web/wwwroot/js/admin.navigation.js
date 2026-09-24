var Admin = Admin || {};

Admin.Navigation = (function () {
    var readTitle = function ($link) {
        var dataTitle = $link.attr("data-nav-title");
        if (dataTitle)
            return dataTitle;

        return $.trim($link.find("p").text());
    };

    var buildMap = function () {
        var map = {};

        var linkElements = $("a.nav-link");

        linkElements.each(function () {
            var $link = $(this);
            var href = $link.attr("href");
            if (!href || href === "#")
                return;

            var title = readTitle($link);
            var parent = $link.attr("data-nav-parent") || null;
            var grandParent = $link.attr("data-nav-grandparent") || null;

            if (!parent) {
                var parents = $link.parentsUntil(".nav-sidebar");
                switch (parents.length) {
                    case 3:
                        {
                            parent = $(parents).eq(2).find("a").find("p").html();
                            if (parent)
                                parent = parent.substring(0, parent.indexOf("<i class"));
                            break;
                        }
                    case 5:
                        {
                            parent = $(parents).eq(2).find("a").find("p").html();
                            grandParent = $(parents).eq(4).find("a").find("p").html();
                            if (parent)
                                parent = parent.substring(0, parent.indexOf("<i class"));
                            if (grandParent)
                                grandParent = grandParent.substring(0, grandParent.indexOf("<i class"));
                            break;
                        }
                    default:
                        break;
                }
            }

            map[href] = { title: title, link: href, parent: parent, grandParent: grandParent };
        });

        return map;
    };

    var map;

    var init = function () {
        map = buildMap();
        var result = [];
        $.ajax({
            cache: false,
            url: rootAppPath + 'Admin/Plugin/AdminNavigationPlugins',
            type: "GET",
            async: false,
            success: function (data, textStatus, jqXHR) {
              result = data;
            }
        });

        for (var i = 0; i < result.length; i++) {
            map[result[i].link] = result[i];
        }
    };
    var events = {};
    return {
        enumerate: function (callback) {
            for (var url in map) {
                var node = map[url];
                callback.call(node, node);
            }
        },
        open: function (url) {
            if (events["open"]) {
                var event = $.Event("open", { url: url });
                events["open"].fire(event);
                if (event.isDefaultPrevented())
                    return;
            }
            window.location.href = url;
        },

        initOnce: function () {
            if (!map)
                init();
        },
        init: init
    };
})();
