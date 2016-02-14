(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterPilotController", RegisterPilotController);

    RegisterPilotController.$inject = ["$state", "$uibModal", "resourceService", "registrationService"];

    function RegisterPilotController($state, $uibModal, resourceService, registrationService) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.registration = {};
        vm.isSubmitting = false;
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

        function submit(isValid) {
            if (!isValid) { return; }

            vm.isSubmitting = true;

            registrationService.registerPilot(vm.registration).then(function(resp){
                $state.go("root.register-pilot.confirmation", { user: resp.data });
            },
            function (resp) {
                vm.isSubmitting = false;
                toast.pop("error", "Invalid Registration", "", resp.data);
            });
        }
    }
})();
