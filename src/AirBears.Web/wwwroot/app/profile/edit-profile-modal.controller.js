(function () {
    "use strict";

    angular
        .module("app")
        .controller("EditProfileModalController", EditProfileModalController);

    EditProfileModalController.$inject = ["$uibModalInstance", "pilotService", "resourceService", "toast", "pilot"];

    function EditProfileModalController($uibModalInstance, pilotService, resourceService, toast, pilot) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.save = save;
        vm.cancel = cancel;

        activate();

        function activate() {
            vm.pilot = pilot;

            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });
        }

        function save(isValid) {
            if (!isValid) { return; }

            vm.isSubmitting = true;

            pilotService.updateProfile(vm.pilot).then(function () {
                toast.pop("success", "Update Success!", "Your pilot profile has been updated.");
                $uibModalInstance.close(vm.pilot);
            }, function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
                vm.isSubmitting = false;
            });
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
