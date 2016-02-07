(function () {
    "use strict";

    angular
        .module("app")
        .factory("authService", authService);

    authService.$inject = ["$http"];

    function authService($http) {
        var service = {};
        var urlBase = "/api/";

        service.authenticate = authenticate;

        return service;

        function authenticate(email, password) {
            return $http.post(urlBase + "token", {email: email, password: password});
        }
    }
})();