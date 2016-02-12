(function () {
    "use strict";

    angular
        .module("app")
        .controller("AuthorityConfirmationController", AuthorityConfirmationController);

    AuthorityConfirmationController.$inject = ["$state", "$stateParams"];

    function AuthorityConfirmationController($state, $stateParams) {
        var vm = this;

        vm.user = $stateParams.user;

        activate();

        function activate() {
            if (!vm.user) {
                $state.go("root.authority-pilot.registration");
            }
        }
    }
})();
