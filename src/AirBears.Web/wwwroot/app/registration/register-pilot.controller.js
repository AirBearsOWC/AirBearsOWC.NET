(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterPilotController", RegisterPilotController);

    RegisterPilotController.$inject = ["resourceService"];

    function RegisterPilotController(resourceService) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.registration = {};
        vm.submit = submit;

        activate();

        function activate() {
            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });
        }

        function submit() {

        }
    }
})();
