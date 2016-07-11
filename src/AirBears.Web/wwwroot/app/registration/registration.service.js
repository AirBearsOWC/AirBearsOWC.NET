(function () {
    "use strict";

    angular
        .module("app")
        .factory("registrationService", registrationService);

    registrationService.$inject = ["$http", "$uibModal"];

    function registrationService($http, $uibModal) {
        var service = {};
        var urlBase = "/api/";

        service.registerPilot = registerPilot;
        service.registerPrepaidPilot = registerPrepaidPilot;
        service.registerAuthority = registerAuthority;
        service.openRegistationOptions = openRegistationOptions;

        return service;

        function registerPilot(registration) {
            return $http.post(urlBase + "accounts/pilot-registration", registration);
        }

        function registerPrepaidPilot(registration) {
            return $http.post(urlBase + "accounts/prepaid-pilot-registration", registration);
        }

        function registerAuthority(registration) {
            return $http.post(urlBase + "accounts/authority-registration", registration);
        }

        function openRegistationOptions() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/registration/register-options-modal.html",
                controller: "RegisterOptionsModalController as vm",
                size: "md"
            });
        }
    }
})();