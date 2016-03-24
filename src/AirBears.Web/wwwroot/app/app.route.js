﻿(function () {
    "use strict";

    angular
        .module("app")
        .config(configureRoutes);

    configureRoutes.$inject = ["$locationProvider", "$stateProvider", "$urlRouterProvider", "$httpProvider", "uiGmapGoogleMapApiProvider"];

    function configureRoutes($locationProvider, $stateProvider, $urlRouterProvider, $httpProvider, uiGmapGoogleMapApiProvider) {
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
            .state("root.pilot-search", {
                url: "/pilot-search",
                templateUrl: "app/pilot-search/pilot-search.html",
                controller: "PilotSearchController as vm"
            })
            .state("root.reset-password", {
                url: "/reset-password?code",
                templateUrl: "app/reset-password/reset-password.html",
                controller: "ResetPasswordController as vm"
            })
            .state("root.profile", {
                url: "/profile",
                templateUrl: "app/profile/profile.html",
                controller: "ProfileController as vm"
            })
            .state("root.manage-pilots", {
                url: "/manage-pilots",
                templateUrl: "app/manage/manage-pilots.html",
                controller: "ManagePilotsController as vm"
            });

        $locationProvider.html5Mode(true);

        $httpProvider.interceptors.push("authInterceptor");

        uiGmapGoogleMapApiProvider.configure({
            // https://github.com/angular-ui/angular-google-maps/blob/master/src/coffee/providers/map-loader.coffee#L84
            //    key: "your api key",
            //v: "3.20", //defaults to latest 3.X anyhow
            //libraries: "weather,geometry,visualization"
            key: "AIzaSyCgM9PF1imC5ExbcVHMBvvvi0wD8wLb8lQ"
        });
    }
})();