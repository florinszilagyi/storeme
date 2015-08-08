(function (window, angular, $) {
    "use strict";
    var app = angular.module('app');

    var controllerId = 'homeController';
    app.controller(controllerId, ['$scope', 'common', '$modal', homeController]);

    function homeController($scope, common, $modal) {
        var vm = this;
        vm.accessCode = null;
        vm.isLoading = false;
        vm.existingDashboard = false;

        function initialize() {
            if (common.$cookies.get('accessCode')) {
                common.$http.post('dashboard/exists', { accessCode: accessCode }).then(function (result) {
                    vm.existingDashboard = true;
                });
            }
        }

        initialize();

        vm.getAccessCode = function () {
            console.log('getting access code');
            if (!vm.accessCode) {
                return common.$http.post('dashboard/new').then(function (result) {
                    vm.accessCode = result.data.accessCode;
                });
            }

            $scope.accessCode = vm.accessCode;
            return vm.accessCode;
        }

        vm.openDashboard = function (accessCode) {
            accessCode = accessCode || common.$cookies.get('accessCode');
            vm.isLoading = true;
            common.$http.post('dashboard/exists', { accessCode: accessCode }).then(function (result) {
                if (result.data.exists) {
                    common.$cookies.put('accessCode', accessCode);
                    common.$location.path('/dashboard'.format(accessCode));
                } else {
                    common.toastr.error('Dashboard does not exist.');
                }

                vm.isLoading = false;
            }, function (result) {
                console.log(result);
                common.toastr.error('Invalid access code');
                vm.isLoading = false;
            });
        }

        vm.newDashboard = function () {
            vm.isLoading = true;
            common.$http.post('dashboard/new').then(function (result) {
                vm.openDashboard(result.data.accessCode);
                vm.isLoading = false;
            }, function (result) {
                vm.isLoading = false;
                console.log(result);
                common.toastr.error('Error creating dashboard...');
            });
        }
    };
})(window, angular, jQuery);
