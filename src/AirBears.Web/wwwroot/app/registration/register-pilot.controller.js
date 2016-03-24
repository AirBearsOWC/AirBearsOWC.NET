(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterPilotController", RegisterPilotController);

    RegisterPilotController.$inject = ["$state", "$uibModal", "braintreeService", "resourceService", "registrationService", "toast"];

    function RegisterPilotController($state, $uibModal, braintreeService, resourceService, registrationService, toast) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.registration = {};
        vm.isSubmitting = false;
        vm.openTermsAndConditions = openTermsAndConditions;
        vm.openPaypal = openPaypal;
        vm.submit = submit;

        activate();

        function activate() {
            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });

            braintreeService.setupHeadlessPaypal(25.00, function (data) {
                vm.registration.nonce = data.nonce;
                vm.paymentDetails = data.details;
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

        function openPaypal() {
            braintreeService.startAuthFlow();
        }

        function submit(isValid) {
            if (!isValid || !vm.registration.nonce) { return; }

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
