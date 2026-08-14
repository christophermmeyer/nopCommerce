/*
** nopCommerce public store color scheme toggle
*/

var DarkMode = {
    storageKey: 'nop.colorScheme',
    className: 'dark-mode',

    isDark: function () {
        return document.documentElement.classList.contains(this.className);
    },

    apply: function (dark) {
        document.documentElement.classList.toggle(this.className, !!dark);
        try {
            localStorage.setItem(this.storageKey, dark ? 'dark' : 'light');
        } catch (e) { }
        this.syncToggle();
    },

    toggle: function () {
        this.apply(!this.isDark());
    },

    syncToggle: function () {
        var buttons = document.querySelectorAll('[data-dark-mode-toggle]');
        var dark = this.isDark();
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].setAttribute('aria-pressed', dark ? 'true' : 'false');
            buttons[i].classList.toggle('is-dark', dark);
            var label = dark ? buttons[i].getAttribute('data-label-disable') : buttons[i].getAttribute('data-label-enable');
            if (label)
                buttons[i].setAttribute('aria-label', label);
        }
    },

    init: function () {
        var self = this;
        this.syncToggle();
        document.addEventListener('click', function (e) {
            var toggle = e.target.closest('[data-dark-mode-toggle]');
            if (!toggle)
                return;
            e.preventDefault();
            self.toggle();
        });
    }
};

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function () {
        DarkMode.init();
    });
} else {
    DarkMode.init();
}
