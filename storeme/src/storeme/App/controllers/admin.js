(function (window, angular, $) {
    "use strict";
    var app = angular.module('app');

    var controllerId = 'adminController';
    app.controller(controllerId, ['$scope', 'common', '$modal', adminController]);

    function adminController($scope, common, $modal) {
        var vm = this;

        vm.getData = function () {
            vm.isLoading = true;
            common.$http.post('admin/getdata').then(function (result) {
                vm.isLoading = false;
                vm.series = ['New dashboards', 'New files'];
                vm.labels = result.data.dates;
                vm.data = [
                    result.data.dashboards,
                    result.data.files
                ];
            }, function (result) {
                vm.isLoading = false;
                console.log(result);
                common.toastr.error('Error loading data...');
            });
        }

        vm.getData();
    };
})(window, angular, jQuery);
