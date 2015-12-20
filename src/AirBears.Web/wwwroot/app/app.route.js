(function () {
    "use strict";

    angular
        .module("app")
        .config(configureRoutes);

    configureRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];

    function configureRoutes($stateProvider, $urlRouterProvider) {
        $urlRouterProvider.otherwise("/");

        $stateProvider
            .state("root", {
                templateUrl: "app/layout/shell.html",
                controller: "ShellController as vm",
                abstract: true
            })
            .state("login", {
                url: "/login",
                templateUrl: "app/auth/login.html",
                controller: "LoginController as vm"
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
            });
    }
})();