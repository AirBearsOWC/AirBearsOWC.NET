(function () {
    "use strict";

    angular
        .module("app")
        .config(configureRoutes);

    configureRoutes.$inject = ["$stateProvider", "$urlRouterProvider", "$httpProvider"];

    function configureRoutes($stateProvider, $urlRouterProvider, $httpProvider) {
        $urlRouterProvider.otherwise("/");

        $stateProvider
            .state("root", {
                templateUrl: "app/layout/shell.html",
                controller: "ShellController as vm",
                abstract: true
            })
            .state("root.home", {
                url: "/",
                templateUrl: "app/dashboard/home.html",
                controller: "HomeController as vm"
            })
            .state("root.register-pilot", {
                url: "/register-pilot",
                templateUrl: "app/registration/register-pilot.html",
                controller: "RegisterPilotController as vm"
            })
            .state("root.register-authority", {
                url: "/register-authority",
                templateUrl: "app/registration/register-authority.html",
                controller: "RegisterAuthorityController as vm"
            })
            .state("root.manage-users", {
                url: "/manage-users",
                templateUrl: "app/manage-users/manage-users.html",
                controller: "ManageUsersController as vm"
            });

        $httpProvider.interceptors.push("authInterceptor");
    }
})();