(function () {
    "use strict";

    angular
        .module("app")
        .controller("PilotConfirmationController", PilotConfirmationController);

    PilotConfirmationController.$inject = ["$state", "$stateParams"];

    function PilotConfirmationController($state, $stateParams) {
        var vm = this;

        vm.user = $stateParams.user;

        activate();

        function activate() {
            if (!vm.user) {
                $state.go("root.register-pilot.registration");
            }
        }
    }
})();
