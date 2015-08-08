(function(window, angular, $) {
    "use strict";
    var app = angular.module('app');

    var controllerId = 'modalDialogController';
    app.controller(controllerId, ['$scope', '$modalInstance', modalDialogController]);

    function modalDialogController($scope, $modalInstance) {
        var vm = this;

        $scope.cancel = function() {
            $modalInstance.dismiss();
        }

        $scope.closeDialog = function() {
            $modalInstance.close();
        }
    };
})(window, angular, jQuery);
