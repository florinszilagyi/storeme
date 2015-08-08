(function (window, angular, $) {
    "use strict";
    angular.module('app').factory('item', ['$http', 'configuration', 'chmod', function ($http, configuration, chmod) {
        var item = function (model, path, accessCode) {
            var rawModel = {
                name: model && model.name || '',
                path: path || [],
                type: model && model.type.toLowerCase() || 'file',
                size: model && model.size || 0,
                date: moment(model && model.date),
                perms: new chmod(model && model.rights),
                content: model && model.content || '',
                recursive: false,
                sizeKb: function () {
                    return Math.round(this.size / 1024, 1);
                },
                fullPath: function () {
                    return ('/' + this.path.join('/') + '/' + this.name).replace(/\/\//, '/');
                }
            };

            this.error = '';
            this.inprocess = false;
            this.accessCode = accessCode;
            this.model = angular.copy(rawModel);
            this.tempModel = angular.copy(rawModel);
        };

        item.prototype.update = function () {
            angular.extend(this.model, angular.copy(this.tempModel));
            return this;
        };

        item.prototype.revert = function () {
            angular.extend(this.tempModel, angular.copy(this.model));
            this.error = '';
            return this;
        };

        item.prototype.defineCallback = function (data, success, error) {
            /* Check if there was some error in a 200 response */
            var self = this;
            if (data.result && data.result.error) {
                self.error = data.result.error;
                return typeof error === 'function' && error(data);
            }
            if (data.error) {
                self.error = data.error.message;
                return typeof error === 'function' && error(data);
            }
            self.update();
            return typeof success === 'function' && success(data);
        };

        item.prototype.createFolder = function (success, error) {
            var self = this;
            var data = {
                path: self.tempModel.path.join('/'),
                name: self.tempModel.name
            };

            if (self.tempModel.name.trim()) {
                self.inprocess = true;
                self.error = '';
                return $http.post(configuration.createFolderUrl, data).success(function (data) {
                    self.defineCallback(data, success, error);
                }).error(function (data) {
                    self.error = data.result && data.result.error ?
                        data.result.error : 'Error creating folder';
                    typeof error === 'function' && error(data);
                })['finally'](function () {
                    self.inprocess = false;
                });
            }
        };

        item.prototype.rename = function (success, error) {
            var self = this;
            var data = {
                "path": self.model.fullPath(),
                "newPath": self.tempModel.fullPath()
            };

            if (self.tempModel.name.trim()) {
                self.inprocess = true;
                self.error = '';
                return $http.post(configuration.renameUrl, data).success(function (data) {
                    self.defineCallback(data, success, error);
                }).error(function (data) {
                    self.error = data.result && data.result.error ?
                        data.result.error : 'Error renaming';
                    typeof error === 'function' && error(data);
                })['finally'](function () {
                    self.inprocess = false;
                });
            }
            return $http.post(configuration.renameUrl, data).success(function (data) {
                self.defineCallback(data, success, error);
            }).error(function (data) {
                self.error = data.result && data.result.error ?
                    data.result.error : 'Error renaming';
                typeof error === 'function' && error(data);
            })['finally'](function () {
                self.inprocess = false;
            });
        };

        item.prototype.copy = function (success, error) {
            var self = this;
            var data = {
                path: self.model.fullPath(),
                newPath: self.tempModel.fullPath()
            };
            if (self.tempModel.name.trim()) {
                self.inprocess = true;
                self.error = '';
                return $http.post(configuration.copyUrl, data).success(function (data) {
                    self.defineCallback(data, success, error);
                }).error(function (data) {
                    self.error = data.result && data.result.error ?
                        data.result.error : 'Error copying';
                    typeof error === 'function' && error(data);
                })['finally'](function () {
                    self.inprocess = false;
                });
            }
            return $http.post(configuration.copyUrl, data).success(function (data) {
                self.defineCallback(data, success, error);
            }).error(function (data) {
                self.error = data.result && data.result.error ?
                    data.result.error : 'Error copying';
                typeof error === 'function' && error(data);
            })['finally'](function () {
                self.inprocess = false;
            });
        };

        item.prototype.download = function (preview) {
            var self = this;
            var data = {
                preview: preview,
                path: self.model.path.join('/'),
                name: self.model.name
            };

            var form = angular.element('<form></form>').attr('action', '/' + configuration.downloadFileUrl).attr('method', 'post');
            // Add the one key/value
            form.append(angular.element("<input></input>").attr('type', 'hidden').attr('name', 'path').attr('value', data.path));
            form.append(angular.element("<input></input>").attr('type', 'hidden').attr('name', 'name').attr('value', data.name));
            form.append(angular.element("<input></input>").attr('type', 'hidden').attr('name', 'accessCode').attr('value', $http.defaults.headers.common.accessCode));
            //send request
            form.appendTo('body').submit().remove();
        }
        item.prototype.preview = function () {
            var self = this;
            return self.download(true);
        };

        item.prototype.getContent = function (success, error) {
            var self = this;
            var data = {
                path: self.tempModel.fullPath()
            };
            self.inprocess = true;
            self.error = '';
            return $http.post(configuration.getContentUrl, data).success(function (data) {
                self.tempModel.content = self.model.content = data.result;
                self.defineCallback(data, success, error);
            }).error(function (data) {
                self.error = data.result && data.result.error ?
                    data.result.error : 'Error getting content';
                typeof error === 'function' && error(data);
            })['finally'](function () {
                self.inprocess = false;
            });
        };

        item.prototype.remove = function (success, error) {
            var self = this;
            var data = {
                path: self.model.path.join('/'),
                name: self.model.name
            };

            self.inprocess = true;
            self.error = '';
            return $http.post(configuration.removeUrl, data).success(function (data) {
                self.defineCallback(data, success, error);
            }).error(function (data) {
                self.error = data.result && data.result.error ?
                    data.result.error : 'Error deleting';
                typeof error === 'function' && error(data);
            })['finally'](function () {
                self.inprocess = false;
            });
        };

        item.prototype.edit = function (success, error) {
            var self = this;
            var data = {
                content: self.tempModel.content,
                path: self.tempModel.fullPath()
            };

            self.inprocess = true;
            self.error = '';

            return $http.post(configuration.editUrl, data).success(function (data) {
                self.defineCallback(data, success, error);
            }).error(function (data) {
                self.error = data.result && data.result.error ?
                    data.result.error : 'Error modifying';
                typeof error === 'function' && error(data);
            })['finally'](function () {
                self.inprocess = false;
            });
        };

        item.prototype.isFolder = function () {
            return this.model.type === 'dir';
        };

        item.prototype.isEditable = function () {
            return !this.isFolder() && configuration.isEditableFilePattern.test(this.model.name);
        };

        item.prototype.isImage = function () {
            return configuration.isImageFilePattern.test(this.model.name);
        };

        return item;
    }]);
})(window, angular, jQuery);
