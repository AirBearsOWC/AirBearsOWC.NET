(function () {
    "use strict";

    angular
        .module("app")
        .controller("ResetPasswordController", ResetPasswordController);

    ResetPasswordController.$inject = ["$state", "$stateParams","toast", "userService", "authService"];

    function ResetPasswordController($state, $stateParams, toast, userService, authService) {
        var vm = this;

        vm.reset = {};
        vm.isSubmitting = false;
        vm.resetSucceeded = false;
        vm.submit = submit;
        vm.openLogin = authService.openLogin;

        activate();

        function activate() {
            if (!$stateParams.code) { $state.go("root.home"); }

            vm.reset.code = $stateParams.code;
        }

        function submit(isValid) {
            if (!isValid) { return; }

            userService.resetPassword(vm.reset).then(function (resp) {
                vm.isSubmitting = false;
                vm.reset = {};
                toast.pop("success", "Success", "Your password has been successfully reset!");
                $state.go("root.home");
            },
            function (resp) {
                vm.isSubmitting = false;
                toast.pop("error", "Error", "An error occurred while attempting to reset your password. It's possible that your recovery link has expired and you need to request a new one.");
            });
        }
    }
})();
