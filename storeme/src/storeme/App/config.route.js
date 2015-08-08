(function(window, angular, $) {
    "use strict";
    var app = angular.module('app');

    app.config(function($stateProvider, $urlRouterProvider) {
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: 'App/Controllers/home.html',
                controller: 'homeController',
                controllerAs: 'vm'
            })
            .state('dashboard', {
                url: '/dashboard',
                templateUrl: 'App/Controllers/dashboard.html',
                controller: 'dashboardController',
                controllerAs: 'vm'
            });
    });

})(window, angular, jQuery);
