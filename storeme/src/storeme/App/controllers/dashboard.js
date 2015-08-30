(function (window, angular, $) {
    "use strict";
    var app = angular.module('app');

    var controllerId = 'dashboardController';
    app.controller(controllerId, ['$scope', 'common', dashboardController]);

    function dashboardController($scope, common) {
        var vm = this;
        vm.accessCode = common.$cookies.get('accessCode');

        function initialize() {
            if (!vm.accessCode) {
                common.$location.path('/403');
            }

            common.$http.post('dashboard/exists', { accessCode: vm.accessCode }).then(function (result) {
                if (!result.data.exists) {
                    common.toastr.error('Dashboard does not exist');
                    common.$location.path('/');
                }
            }, function () {
                common.toastr.error('Dashboard does not exist');
                common.$location.path('/');
            });

            common.initializeController(controllerId, []);

            common.$timeout(function () {
                if (!common.$cookies.get('hintShown')) {
                    common.toastr.success("Click on 'View access code' in the top bar to copy it to your clipboard", "Save your access code");
                    common.$cookies.put('hintShown', true);
                }
            }, 2000);
        }
        initialize();

        vm.onMouseOver = function () {
            vm.mouseOver = true;
        }

        vm.onMouseLeave = function () {
            vm.mouseOver = false;
        }

        vm.logout = function () {
            var instance = common.$modal.open({
                templateUrl: 'logout.html',
                controller: function ($scope, $modalInstance) {
                    $scope.ok = function () {
                        $modalInstance.close();
                    }

                    $scope.cancel = function () {
                        $modalInstance.dismiss();
                    }

                    $scope.copy = function () {
                        vm.copyToClipboard();
                    }
                }
            });

            instance.result.then(function () {
                common.$cookies.remove('accessCode');
                common.$location.path('/');
                common.$rootScope.$broadcast('app.logout');
            });
        }

        vm.copyToClipboard = function () {
            common.copyToClipboard(vm.accessCode);
            common.toastr.info('Access code copied to clipboard');
        }
    };
})(window, angular, jQuery);
