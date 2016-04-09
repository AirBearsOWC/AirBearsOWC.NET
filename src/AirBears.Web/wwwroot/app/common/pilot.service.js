(function () {
    "use strict";

    angular
        .module("app")
        .factory("pilotService", pilotService);

    pilotService.$inject = ["$http", "$q"];

    function pilotService($http, $q) {
        var service = {};
        var urlBase = "/api/pilots";

        service.search = search;
        service.getPilots = getPilots;
        service.updateProfile = updateProfile;
        service.markTeeShirtMailed = markTeeShirtMailed;

        return service;

        function search(criteria) {
            return $http.post(urlBase + "/search", criteria).then(function (resp) {
                return resp.data;
            }, function (resp) {
                return $q.reject(resp);
            });
        }

        function getPilots() {
            return $http.get(urlBase).then(function (resp) {
                return resp.data;
            });
        }

        function updateProfile(pilot) {
            return $http.put(urlBase + "/me", pilot).then(function (resp) {
                return resp.data;
            }, function (resp) {
                return $q.reject(resp);
            });
        }

        function markTeeShirtMailed(userId, isMailed) {
            return $http.put(urlBase + "/" + userId + "/tee-shirt-mailed", isMailed);
        }
    }
})();