(function () {
    "use strict";

    angular
        .module("app")
        .controller("ShellController", ShellController);

    ShellController.$inject = ["$uibModal"]; 

    function ShellController($uibModal) {
        var vm = this;

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
                size: "md"
            });

            modalInstance.result.then(function (result) {

            });
        }
    }
})();
