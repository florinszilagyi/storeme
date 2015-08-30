(function(window, angular, $) {
    "use strict";

    angular.module('app')
        .controller('fileManagerController', ['$scope', 'common', 'item', 'fileNavigator', '$state', fileManagerController]);

    function fileManagerController($scope, common, item, fileNavigator){
        $scope.config = common.configuration;
        $scope.appName = common.configuration.appName;
        $scope.orderProp = ['model.type', 'model.name'];
        $scope.query = '';
        $scope.temp = new item();
        $scope.fileNavigator = new fileNavigator();
        $scope.uploadFileList = [];
        $scope.viewTemplate = common.$cookies.viewTemplate || 'main-table.html';
        $scope.fileNavigator.accessCode = $scope.accessCode;
        common.$http.defaults.headers.common['accessCode'] = $scope.accessCode;

        $scope.setTemplate = function (name) {
            $scope.viewTemplate = common.$cookies.viewTemplate = name;
        };

        $scope.touch = function (it) {
            it = it || new item(undefined, undefined, $scope.accessCode);
            it.revert && it.revert();
            $scope.temp = it;
        };

        $scope.smartRightClick = function (it) {
            $scope.touch(it);
        };

        $scope.smartClick = function (item) {
            if (item.isFolder()) {
                return $scope.fileNavigator.folderClick(item);
            };
            if (item.isImage()) {
                return item.preview();
            }
            if (item.isEditable()) {
                item.getContent();
                $scope.touch(item);
                $('#edit').modal('show');
                return;
            }
        };

        $scope.remove = function (it) {
            it.remove(function () {
                $scope.fileNavigator.refresh();
                $('#delete').modal('hide');
            });
        };

        $scope.createFolder = function (it) {
            var name = it.tempModel.name && it.tempModel.name.trim();
            it.tempModel.type = 'dir';
            it.tempModel.path = $scope.fileNavigator.currentPath;
            if (name && !$scope.fileNavigator.fileNameExists(name)) {
                it.createFolder(function () {
                    $scope.fileNavigator.refresh();
                    $('#newfolder').modal('hide');
                });
            } else {
                $scope.temp.error = 'Invalid filename';
                return false;
            }

            return true;
        };

        $scope.uploadFile = function (it) {
            var modalInstance = common.$modal.open({
                animation: true,
                templateUrl: 'uploadModal.html',
                size: 'lg',
                scope: $scope,
                controller: 'modalDialogController'
            });

            modalInstance.result.then(function() {
                common.$state.forceReload();
            });
        }

        $scope.getQueryParam = function (param) {
            var found;
            window.location.search.substr(1).split("&").forEach(function (item) {
                if (param === item.split("=")[0]) {
                    found = item.split("=")[1];
                }
            });
            return found;
        };

        $scope.isWindows = $scope.getQueryParam('server') === 'Windows';
        $scope.fileNavigator.refresh();
    }
})(window, angular, jQuery);
