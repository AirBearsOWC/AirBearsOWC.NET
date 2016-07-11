(function () {
    "use strict";

    angular
        .module("app")
        .controller("PrepaidRegistrationController", PrepaidRegistrationController);

    PrepaidRegistrationController.$inject = ["$state", "$uibModal", "resourceService", "registrationService", "toast"];

    function PrepaidRegistrationController($state, $uibModal, resourceService, registrationService, toast) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.registration = {};
        vm.isSubmitting = false;
        vm.isRegistrationComplete = false;
        vm.openTermsAndConditions = openTermsAndConditions;
        vm.submit = submit;
        vm.done = done;

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

        function submit(form) {
            if (!form.$valid) { return; }

            vm.isSubmitting = true;
            vm.registration.nonce = "PREPAID";

            registrationService.registerPrepaidPilot(vm.registration).then(function (resp) {
                vm.isSubmitting = false;
                vm.isRegistrationComplete = true;
                form.$setPristine();
            },
            function (resp) {
                vm.isSubmitting = false;
                toast.pop("error", "Invalid Registration", "", resp.data);
            });
        }

        function done() {
            vm.isRegistrationComplete = false;
            vm.isSubmitting = false;
            vm.registration = {};
        }
    }
})();
