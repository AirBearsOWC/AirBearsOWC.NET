(function () {
    "use strict";

    angular
        .module("app")
        .controller("ProfileController", ProfileController);

    ProfileController.$inject = ["$uibModal", "resourceService", "userService", "authService", "toast"];

    function ProfileController($uibModal, resourceService, userService, authService, toast) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.pilot = {};

        vm.openChangePasswordModal = openChangePasswordModal;

        activate();

        function activate() {
            getCurrentUser();

            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });
        }

        function getCurrentUser() {
            userService.getCurrentUser().then(function (user) {
                vm.pilot = user;
            }, function () {
                authService.openLogin(function () {
                    getCurrentUser();
                });
            });
        }

        function openChangePasswordModal(callback) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/profile/change-password-modal.html",
                controller: "ChangePasswordModalController as vm",
                size: "md"
            });
        }
    }
})();
