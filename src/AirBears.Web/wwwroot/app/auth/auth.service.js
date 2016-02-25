(function () {
    "use strict";

    angular
        .module("app")
        .factory("authService", authService);

    authService.$inject = ["$http", "store"];

    function authService($http, store) {
        var service = {};
        var urlBase = "/api/";

        service.authenticate = authenticate;
        service.logout = logout;
        service.getAuthToken = getAuthToken;

        return service;

        function authenticate(email, password) {
            return $http.post(urlBase + "token", { email: email, password: password })
                .success(function (resp) {
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
    }
})();