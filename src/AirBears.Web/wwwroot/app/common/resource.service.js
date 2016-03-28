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
        service.getPayloads = getPayloads;
        service.getFlightTimes = getFlightTimes;

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

        function getPayloads() {
            return $http.get(urlBase + "payloads").then(function (resp) {
                return resp.data;
            });
        }

        function getFlightTimes() {
            return $http.get(urlBase + "flight-times").then(function (resp) {
                return resp.data;
            });
        }
    }
})();