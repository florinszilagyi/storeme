(function(window, angular, $) {
    "use strict";
    var app = angular.module('app');

    app.filter('strLimit', ['$filter', function ($filter) {
        return function (input, limit) {
            if (input.length <= limit) {
                return input;
            }
            return $filter('limitTo')(input, limit) + '...';
        };
    }]);

    /**
     * jQuery inits
     */
    var menuSelectors = '.main-navigation .table-files td a, .iconset a.thumbnail';

    $(window.document).on('shown.bs.modal', '.modal', function () {
        var self = this;
        var timer = setTimeout(function () {
            $('[autofocus]', self).focus();
            timer && clearTimeout(timer);
        }, 100);
    });

    $(window.document).on('click', function () {
        $("#context-menu").hide();
    });

    $(window.document).on('contextmenu', menuSelectors, function (e) {
        $("#context-menu").hide().css({
            left: e.pageX,
            top: e.pageY
        }).show();
        e.preventDefault();
    });

// ReSharper disable once NativeTypePrototypeExtending
    String.prototype.format = function () {
        var formatted = this;
        for (var i = 0; i < arguments.length; i++) {
            var regexp = new RegExp('\\{' + i + '\\}', 'gi');
            formatted = formatted.replace(regexp, arguments[i]);
        }
        return formatted;
    };

})(window, angular, jQuery);
