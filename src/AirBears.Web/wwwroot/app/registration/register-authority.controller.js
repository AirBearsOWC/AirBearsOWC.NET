(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterAuthorityController", RegisterAuthorityController);

    RegisterAuthorityController.$inject = ["$state", "registrationService", "toast"];

    function RegisterAuthorityController($state, registrationService, toast) {
        var vm = this;

        vm.registration = {};
        vm.isSubmitting = false;
        vm.submit = submit;

        activate();

        function activate() {

        }

        function submit(isValid) {
            if (!isValid) { return; }
            
            vm.isSubmitting = true;

            registrationService.registerAuthority(vm.registration).then(function (resp) {
                $state.go("root.register-authority.confirmation", { user: resp.data });
            }, 
            function (resp) {
                vm.isSubmitting = false;
                toast.pop("error", "Invalid Registration", "", resp.data);
            });
        }
    }
})();
