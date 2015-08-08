(function(angular) {
    "use strict";
    angular.module('app').constant("configuration", {
        listUrl: "Dashboard/Files",
        uploadUrl: "Dashboard/Upload",
        renameUrl: "Home/NotImplemented",
        copyUrl: "Home/NotImplemented",
        removeUrl: "Dashboard/Delete",
        editUrl: "Home/NotImplemented",
        getContentUrl: "Home/NotImplemented",
        createFolderUrl: "Dashboard/NewFolder",
        downloadFileUrl: "Dashboard/Download",

        allowedActions: {
            rename: false,
            copy: false,
            edit: false,
            download: true,
            preview: false,
            remove: true
        },

        isEditableFilePattern: /\.(txt|html?|aspx?|ini|pl|py|md|css|js|log|htaccess|htpasswd|json|sql|xml|xslt?|sh|rb|as|bat|cmd|coffee|php[3-6]?|java|c|cbl|go|h|scala|vb)$/i,
        isImageFilePattern: /\.(jpe?g|gif|bmp|png|svg|tiff?)$/i,
        isExtractableFilePattern: /\.(gz|tar|rar|g?zip)$/i,
        templatePath: 'App/directives/templates'
    });

    angular.module('app').config(function (toastrConfig) {
        angular.extend(toastrConfig, {
            allowHtml: false,
            autoDismiss: false,
            closeButton: false,
            closeHtml: '<button>&times;</button>',
            containerId: 'toast-container',
            extendedTimeOut: 1000,
            iconClasses: {
                error: 'toast-error',
                info: 'toast-info',
                success: 'toast-success',
                warning: 'toast-warning'
            },
            maxOpened: 3,
            messageClass: 'toast-message',
            newestOnTop: true,
            onHidden: null,
            onShown: null,
            positionClass: 'toast-bottom-right',
            preventDuplicates: false,
            preventOpenDuplicates: false,
            progressBar: false,
            tapToDismiss: true,
            target: 'body',
            templates: {
                toast: 'directives/toast/toast.html',
                progressbar: 'directives/progressbar/progressbar.html'
            },
            timeOut: 7500,
            titleClass: 'toast-title',
            toastClass: 'toast'
        });
    });
})(angular);
