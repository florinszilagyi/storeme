(function(angular, $) {
    "use strict";
    angular.module('app').controller('modalController', [
        '$scope', '$rootScope', 'configuration', 'fileNavigator',
        function($scope, $rootScope, configuration, fileNavigator) {

        $scope.appName = configuration.appName;
        $scope.orderProp = ['model.type', 'model.name'];
        $scope.fileNavigator = new fileNavigator();

        $rootScope.select = function(item, temp) {
            temp.tempModel.path = item.model.fullPath().split('/');
            $('#selector').modal('hide');
        };

        $rootScope.openNavigator = function(item) {
            $scope.fileNavigator.currentPath = item.model.path.slice();
            $scope.fileNavigator.refresh();
            $('#selector').modal('show');
        };
    }]);
})(angular, jQuery);