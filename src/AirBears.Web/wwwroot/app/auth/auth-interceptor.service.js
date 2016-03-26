(function () {
    "use strict";

    angular
        .module("app")
        .factory("authInterceptor", authInterceptor);

    authInterceptor.$inject = ["$rootScope", "$location", "$q", "store", "toast"];

    function authInterceptor($rootScope, $location, $q, store, toast) {
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

        function responseError(response) {
            if (response.status === 401) {
                $rootScope.$broadcast("UNAUTHENTICATED");
                store.remove("auth_token");
                $location.path("/");
                toast.pop("warning", "Session Expired", "Your session has ended. Please log in to access your Air Bears account.");
            }

            return $q.reject(response);
        }
    }
})();