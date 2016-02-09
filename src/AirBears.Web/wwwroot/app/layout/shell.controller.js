(function () {
    "use strict";

    angular
        .module("app")
        .controller("ShellController", ShellController);

    ShellController.$inject = ["$uibModal", "userService"]; 

    function ShellController($uibModal, userService) {
        var vm = this;

        vm.user = null;
        vm.openRegistationOptions = openRegistationOptions;
        vm.openLogin = openLogin;

        activate();

        function activate() { }

        function openRegistationOptions() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/registration/register-options-modal.html",
                controller: "RegisterOptionsModalController as vm",
                size: "md"
            });

            modalInstance.result.then(function (result) {
                
            });
        }

        function openLogin() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/auth/login-modal.html",
                controller: "LoginModalController as vm",
                size: "sm"
            });

            modalInstance.result.then(function (result) {
                if (result.loginSuccess) {
                    getCurrentUser();
                }
            });
        }

        function getCurrentUser() {
            return userService.getCurrentUser().then(function (user) {
                vm.user = user;
            });
        }
    }
})();
