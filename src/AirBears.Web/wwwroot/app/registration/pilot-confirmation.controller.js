(function () {
    "use strict";

    angular
        .module("app")
        .controller("PilotConfirmationController", PilotConfirmationController);

    PilotConfirmationController.$inject = ["$state", "$stateParams", "authService"];

    function PilotConfirmationController($state, $stateParams, authService) {
        var vm = this;

        vm.user = $stateParams.user;
        vm.openLogin = authService.openLogin;

        activate();

        function activate() {
            if (!vm.user) {
                $state.go("root.register-pilot.registration");
            }
        }
    }
})();
