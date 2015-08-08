/*!
 * Angular FileManager v1.0.1 (https://github.com/joni2back/angular-filemanager)
 * Jonas Sciangula Street <joni2back@gmail.com>
 * Licensed under MIT (https://github.com/joni2back/angular-filemanager/blob/master/LICENSE)
 */

(function(window, angular, $) {
    "use strict";
    var app = angular.module('app');

    app.directive('fileManager', ['$parse', 'configuration', function($parse, configuration) {
        return {
            restrict: 'EA',
            templateUrl: configuration.templatePath + '/fileManager.html',
            scope: {
                accessCode: '='
            }
        };
    }]);
})(window, angular, jQuery);