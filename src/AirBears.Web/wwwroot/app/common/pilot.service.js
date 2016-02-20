(function () {
    "use strict";

    angular
        .module("app")
        .factory("pilotService", pilotService);

    pilotService.$inject = ["$http"];

    function pilotService($http) {
        var service = {};
        var urlBase = "/api/pilots/";

        service.search = search;

        return service;

        function search(address, distance) {
            return $http.get(urlBase + "search?address=" + address + "&distance=" + distance);
        }
    }
})();