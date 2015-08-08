(function(window, angular) {
    "use strict";
    angular.module('app').service('fileUploader', ['$http', 'configuration', function ($http, configuration) {
        var self = this;
        self.requesting = false; 
        self.upload = function(fileList, path, accessCode) {
            var form = new window.FormData();  
            form.append('uploadPath', '/' + path.join('/'));

            for (var i = 0; i < fileList.length; i++) {
                var fileObj = fileList.item(i);
                fileObj instanceof window.File && form.append('file-' + i, fileObj);
            }

            self.requesting = true;
            return $http.post(configuration.uploadUrl, form, {
                transformRequest: angular.identity,
                headers: {
                    "Content-Type": undefined,
                    accessCode: accessCode
                }
            }).success(function(data) {
                self.inprocess = false;
            }).error(function(data) {
                self.inprocess = false;
            });
        };
    }]);
})(window, angular);