(function () {
    "use strict";

    angular
        .module("app")
        .factory("registrationService", registrationService);

    registrationService.$inject = ["$http"];

    function registrationService($http) {
        var service = {};
        var urlBase = "/api/";

        service.registerPilot = registerPilot;

        return service;

        function registerPilot(registration) {
            return $http.post(urlBase + "accounts/registration", registration);
        }
    }
})();