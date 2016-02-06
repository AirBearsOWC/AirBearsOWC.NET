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
        service.registerAuthority = registerAuthority;

        return service;

        function registerPilot(registration) {
            return $http.post(urlBase + "accounts/pilot-registration", registration);
        }

        function registerAuthority(registration) {
            return $http.post(urlBase + "accounts/authority-registration", registration);
        }
    }
})();