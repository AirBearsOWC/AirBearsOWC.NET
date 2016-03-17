(function () {
    "use strict";

    angular
        .module("app")
        .controller("ProfileController", ProfileController);

    ProfileController.$inject = ["$state", "$uibModal", "resourceService", "userService", "toast"];

    function ProfileController($state, $uibModal, resourceService, userService, toast) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.pilot = {};

        activate();

        function activate() {
            userService.getCurrentUser().then(function (user) {
                vm.pilot = user;
            });

            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });
        }
    }
})();
