(function () {
    "use strict";

    angular
        .module("app")
        .factory("authInterceptor", authInterceptor);

    authInterceptor.$inject = ["$rootScope", "store"];

    function authInterceptor($rootScope, store) {
        var service = {};
        var urlBase = "/api/";

        service.request = request;
        service.responseError = responseError;

        return service;

        function request(config) {
            var token = store.get("auth_token");

            if (token) {
                config.headers.authorization = "Bearer " + token;
            }

            return config;
        }

        function responseError(){
            if (response.status === 401) {
                $rootScope.$broadcast("unauthorized");
            }
            return response;
        }
    }
})();