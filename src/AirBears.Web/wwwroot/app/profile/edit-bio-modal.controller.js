(function () {
    "use strict";

    angular
        .module("app")
        .controller("EditBioModalController", EditBioModalController);

    EditBioModalController.$inject = ["$uibModalInstance", "pilotService", "toast", "pilot"];

    function EditBioModalController($uibModalInstance, pilotService, toast, pilot) {
        var vm = this;

        vm.save = save;
        vm.cancel = cancel;

        activate();

        function activate() {
            vm.pilot = pilot;
        }

        function save(isValid) {
            if (!isValid) { return; }

            vm.isSubmitting = true;

            pilotService.updateProfile(vm.pilot).then(function () {
                toast.pop("success", "Update Success!", "Your pilot bio has been updated.");
                $uibModalInstance.close(vm.pilot.bio);
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
