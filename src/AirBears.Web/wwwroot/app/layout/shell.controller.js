(function () {
    "use strict";

    angular
        .module("app")
        .controller("ShellController", ShellController);

    ShellController.$inject = ["$state", "$uibModal", "authService", "userService"]; 

    function ShellController($state, $uibModal, authService, userService) {
        var vm = this;

        vm.user = null;
        vm.openRegistationOptions = openRegistationOptions;
        vm.openLogin = openLogin;
        vm.logout = logout;

        activate();

        function activate() {
            getCurrentUser();
        }

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

        function logout() {
            authService.logout();
            vm.user = null;
            $state.go("root.home");
        }

        function getCurrentUser() {
            return userService.getCurrentUser().then(function (user) {
                vm.user = user;
            });
        }
    }
})();
