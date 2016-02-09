(function () {
    "use strict";

    angular
        .module("app")
        .factory("userService", userService);

    userService.$inject = ["$http"];

    function userService($http) {
        var service = {};
        var urlBase = "/api/users/";

        service.getCurrentUser = getCurrentUser;

        return service;

        function getCurrentUser() {
            return $http.get("/api/me").then(function (resp) {
                return resp.data;
            });
        }
    }
})();