﻿(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterAuthorityController", RegisterAuthorityController);

    RegisterAuthorityController.$inject = ["$state", "registrationService"];

    function RegisterAuthorityController($state, registrationService) {
        var vm = this;

        vm.registration = {};
        vm.submit = submit;

        activate();

        function activate() {

        }

        function submit(isValid) {
            if (!isValid) { return; }

            registrationService.registerAuthority(vm.registration).then(function (resp) {
                $state.go("root.register-authority.confirmation", { user: resp.data });
            });
        }
    }
})();
