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

        function getUsers(page, pageSize, name, onlyUnapproved, sortBy, ascending) {
            var url = urlBase + "?page=" + page + "&pageSize=" + pageSize;

            if (name) { url += "&name=" + name; }
            if (onlyUnapproved) { url += "&onlyUnapproved=" + onlyUnapproved; }
            if (sortBy) { url += "&sortBy=" + sortBy; }
            if (!ascending) { url += "&ascending=" + ascending; }

            return $http.get(url).then(function (resp) {
                return resp.data;
            }, function (resp) {
                return $q.reject(resp);
            });
        }

        function markApproved(userId, isApproved) {
            return $http.put(urlBase + "/" + userId + "/approve", isApproved);
        }
    }
})();