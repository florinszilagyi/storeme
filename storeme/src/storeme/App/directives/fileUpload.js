(function (window, angular, $) {
    "use strict";
    var app = angular.module('app');

    app.directive('fileUpload', ['configuration', function (configuration) {
        return {
            restrict: 'EA',
            templateUrl: configuration.templatePath + '/fileUpload.html',
            controller: ['$scope', '$q', 'FileUploader', fileUploadController],
            scope: {
                accessCode: '=',
                onUploadComplete: '&',
                onUploadStart: '&',
                uploadPath: '='
            },
            link: function () {
                $("#filedropzone").click(function () {
                    $("#uploadfileinput").click();
                });
            }
        };
    }]);

    function fileUploadController($scope, $q, fileUploader) {
        $scope.files = [];
        $scope.errors = false;
        $scope.completed = false;

        $scope.uploader = new fileUploader({
            url: 'dashboard/upload'
        });

        $scope.uploader.onBeforeUploadItem = function (item) {
            item.headers.accessCode = $scope.accessCode;
            item.headers.uploadPath = $scope.uploadPath;
        }

        $scope.uploader.onCompleteAll = function () {
            if (!$scope.errors) {
                $scope.onUploadComplete && $scope.onUploadComplete();
            }

            $scope.completed = true;
        }

        $scope.uploader.onCompleteItem = function(item, response, status, headers) {
            if (response.result === "error") {
                item.isError = true;
                item.isSuccess = false;
                item.message = response.message;
                $scope.errors = true;
            }
            else {
                item.isSuccess = true;
                item.message = "Complete";
            }
        }

        $scope.startUpload = function () {
            $q.when($scope.onUploadStart && $scope.onUploadStart())
                .then(function () {
                    $scope.uploader.uploadAll();
                });
        }
    };
})(window, angular, jQuery);
