(function () {
    "use strict";

    angular
        .module("app")
        .controller("ChangePasswordController", ChangePasswordController);

    ChangePasswordController.$inject = ["$uibModalInstance", "userService", "toast"];

    function ChangePasswordController($uibModalInstance, userService, toast) {
        var vm = this;

        vm.save = save;
        vm.cancel = cancel;

        activate();

        function activate() { }

        function save(isValid) {
            if (!isValid) { return; }

            vm.isSubmitting = true;

            userService.changePassword(vm.model).then(function () {
                toast.pop("success", "Save Successful!", "Your password was successfully changed.");
                $uibModalInstance.close();
            }, function (resp) {
                toast.pop("error", "Error", "", resp.data);
                vm.isSubmitting = false;
            });
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
