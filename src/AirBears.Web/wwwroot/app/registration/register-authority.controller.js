(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterAuthorityController", RegisterAuthorityController);

    RegisterAuthorityController.$inject = [];

    function RegisterAuthorityController() {
        var vm = this;

        vm.registration = {};
        vm.submit = submit;

        activate();

        function activate() {

        }

        function submit() {
            registrationService.registerAuthority(vm.registration).then(function (data) {

            });
        }
    }
})();
