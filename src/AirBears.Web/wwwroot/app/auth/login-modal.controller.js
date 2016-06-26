(function () {
    "use strict";

    angular
        .module("app")
        .controller("LoginModalController", LoginModalController);

    LoginModalController.$inject = ["$uibModalInstance", "authService", "userService", "toast"];

    function LoginModalController($uibModalInstance, authService, userService, toast) {
        var vm = this;

        vm.showRecoverPassword = false;

        vm.login = login;
        vm.clear = clear;
        vm.cancel = cancel;
        vm.toggleRecoverPassword = toggleRecoverPassword;
        vm.recoverPassword = recoverPassword;

        activate();

        function activate() { }

        function login(isValid) {
            if (!isValid) { return; }

            vm.isSubmitting = true;

            authService.authenticate(vm.username, vm.password).then(function () {
                $uibModalInstance.close({ loginSuccess: true });
            }, function (resp) {
                vm.message = null;
                vm.password = null;
                if (resp && resp.data) {
                    vm.message = resp.data;
                }
                else {
                    vm.message = "An error occured while logging in.";
                }
                vm.isSubmitting = false;
            });
        }

        function clear() {
            vm.message = null;
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }

        function toggleRecoverPassword() {
            vm.message = null;
            vm.showRecoverPassword = !vm.showRecoverPassword;
        }

        function recoverPassword(isValid) {
            if (!isValid) { return; }

            userService.recoverPassword(vm.username, vm.captchaResponse).then(function () {
                toast.pop("success", "Email Sent!", "Check your email for a password recovery link.");
                $uibModalInstance.close({ loginSuccess: false });
            }, function (resp) {
                vm.message = null;
                if (resp && resp.data) {
                    vm.message = resp.data;
                }
                else {
                    vm.message = "An error occured while recovering your password.";
                }
                vm.isSubmitting = false;
            });
        }
    }
})();
