(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterAuthorityController", RegisterAuthorityController);

    RegisterAuthorityController.$inject = ["registrationService"];

    function RegisterAuthorityController(registrationService) {
        var vm = this;

        vm.registration = {};
        vm.submit = submit;

        activate();

        function activate() {

        }

        function submit(isValid) {
            if (!isValid) { return; }

            registrationService.registerAuthority(vm.registration).then(function (data) {
                $state.go("root.register-authority.confirmation", { user: data });
            });
        }
    }
})();
