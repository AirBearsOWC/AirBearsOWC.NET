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

        function search(address, distance, latitude, longitude) {
            // Send coordinates if the address is null.
            if (!address) {
                return $http.get(urlBase + "search?distance=" + distance + "&latitude=" + latitude + "&longitude=" + longitude);
            }

            // Otherwise send the address.
            return $http.get(urlBase + "search?address=" + address + "&distance=" + distance);
        }
    }
})();