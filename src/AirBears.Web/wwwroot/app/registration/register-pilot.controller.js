﻿(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterPilotController", RegisterPilotController);

    RegisterPilotController.$inject = ["$uibModal", "resourceService", "registrationService"];

    function RegisterPilotController($uibModal, resourceService, registrationService) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.registration = {};
        vm.openTermsAndConditions = openTermsAndConditions;
        vm.submit = submit;

        activate();

        function activate() {
            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });
        }

        function openTermsAndConditions() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/registration/terms.html",
                controller: "TermsController as vm",
                size: "lg"
            });

            modalInstance.result.then(function (result) {
                if (result.hasAgreed) { vm.registration.hasAgreedToTerms = true; }
            });
        }

        function submit() {
            registrationService.registerPilot(vm.registration).then(function(data){

            });
        }
    }
})();
