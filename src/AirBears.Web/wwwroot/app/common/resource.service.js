(function () {
    "use strict";

    angular
        .module("app")
        .factory("resourceService", resourceService);

    resourceService.$inject = ["$http"];

    function resourceService($http) {
        var service = {};
        var urlBase = "/api/resources/";

        service.getStates = getStates;
        service.getTeeShirtSizes = getTeeShirtSizes;

        return service;

        function getStates() {
            return $http.get(urlBase + "states").then(function (resp) {
                return resp.data;
            });
        }

        function getTeeShirtSizes() {
            return $http.get(urlBase + "tee-shirt-sizes").then(function (resp) {
                return resp.data;
            });
        }
    }
})();