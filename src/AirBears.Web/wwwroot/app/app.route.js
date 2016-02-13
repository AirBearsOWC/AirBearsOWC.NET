﻿(function () {
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
                abstract: true,
                templateUrl: "app/registration/pilot-registration-shell.html",
            })
            .state("root.register-pilot.registration", {
                url: "/register-pilot",
                templateUrl: "app/registration/register-pilot.html",
                controller: "RegisterPilotController as vm"
            })
            .state("root.register-pilot.confirmation", {
                url: "/register-pilot-confirmation",
                templateUrl: "app/registration/pilot-confirmation.html",
                controller: "PilotConfirmationController as vm",
                params: {
                    user: null
                }
            })
            .state("root.register-authority", {
                abstract: true,
                templateUrl: "app/registration/authority-registration-shell.html"
            })
            .state("root.register-authority.registration", {
                url: "/register-authority",
                templateUrl: "app/registration/register-authority.html",
                controller: "RegisterAuthorityController as vm"
            })
            .state("root.register-authority.confirmation", {
                url: "/register-authority-confirmation",
                templateUrl: "app/registration/authority-confirmation.html",
                controller: "AuthorityConfirmationController as vm",
                params: {
                    user: null
                }
            })
            .state("root.manage-users", {
                url: "/manage-users",
                templateUrl: "app/manage-users/manage-users.html",
                controller: "ManageUsersController as vm"
            });

        $httpProvider.interceptors.push("authInterceptor");
    }
})();