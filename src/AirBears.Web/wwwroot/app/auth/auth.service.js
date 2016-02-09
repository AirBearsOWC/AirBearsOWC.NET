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

        return service;

        function authenticate(email, password) {
            return $http.post(urlBase + "token", {email: email, password: password}).success(function(resp){
                var token = resp.data.token;
                store.set("auth_token", token);
            });
        }
    }
})();