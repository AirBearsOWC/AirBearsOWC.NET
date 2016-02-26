﻿(function () {
    "use strict";

    angular
        .module("app")
        .factory("authService", authService);

    authService.$inject = ["$rootScope", "$http", "store"];

    function authService($rootScope, $http, store) {
        var service = {};
        var urlBase = "/api/";

        service.authenticate = authenticate;
        service.logout = logout;
        service.getAuthToken = getAuthToken;
        service.onLogin = onLogin;

        return service;

        function authenticate(email, password) {
            return $http.post(urlBase + "token", { email: email, password: password })
                .success(function (resp) {
                    $rootScope.$broadcast("LOG_IN");
                    store.set("auth_token", resp.token);
                }).error(function (resp) {
                    return resp;
                });
        }

        function logout() {
            store.remove("auth_token");
        }

        function getAuthToken() {
            return store.get("auth_token");
        }

        function onLogin(scope, callback) {
            var handler = $rootScope.$on("LOG_IN", callback);
            scope.$on("$destroy", handler);
        }
    }
})();