(function () {
    "use strict";

    angular
        .module("app")
        .controller("LoginModalController", LoginModalController);

    LoginModalController.$inject = ["$modalInstance", "authService"];

    function LoginModalController($modalInstance, authService) {
        var vm = this;

        vm.messages = [];
        vm.login = login;
        vm.clear = clear;
        vm.cancel = cancel;

        activate();

        function activate() { }

        function login() {
            vm.isLoggingIn = true;

            authService.authenticate(vm.username, vm.password).then(function () {
                $modalInstance.close({ loginSuccess: true });
            },
            function (resp) {
                vm.messages = [];
                if (resp && resp.data) {
                    vm.message = resp.data;
                }
                else {
                    vm.message = "An error occured while logging in.";
                }
                vm.isLoggingIn = false;
            });
        }

        function clear() {
            vm.message = null;
        }

        function cancel() {
            $modalInstance.dismiss("cancel");
        }
    }
})();
