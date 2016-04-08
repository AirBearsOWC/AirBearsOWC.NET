(function () {
    "use strict";

    angular
        .module("app")
        .factory("authorityService", authorityService);

    authorityService.$inject = ["$http", "$q"];

    function authorityService($http, $q) {
        var service = {};
        var urlBase = "/api/authority-users";

        service.getUsers = getUsers;
        service.markApproved = markApproved;

        return service;

        function getUsers() {
            return $http.get(urlBase).then(function (resp) {
                return resp.data;
            });
        }

        function markApproved(userId, isApproved) {
            return $http.put(urlBase + "/" + userId + "/approve", isApproved);
        }
    }
})();