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
        service.getPilot = getPilot;
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

        function getPilot(id) {
            return $http.get(urlBase + "/" + id).then(function (resp) {
                return resp.data;
            }, function (resp) {
                return $q.reject(resp);
            });
        }

        function getPilots(page, pageSize, name, onlyShirtsNotSent, sortBy, ascending) {
            var url = urlBase + "?page=" + page + "&pageSize=" + pageSize;

            if (name) { url += "&name=" + name; }
            if (onlyShirtsNotSent) { url += "&onlyShirtsNotSent=" + onlyShirtsNotSent; }
            if (sortBy) { url += "&sortBy=" + sortBy; }
            if (!ascending) { url += "&ascending=" + ascending; }

            return $http.get(url).then(function (resp) {
                return resp.data;
            }, function (resp) {
                return $q.reject(resp);
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