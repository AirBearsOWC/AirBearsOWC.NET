(function () {
    "use strict";

    angular
        .module("app")
        .controller("LoginModalController", LoginModalController);

    LoginModalController.$inject = ["$uibModalInstance"];

    function LoginModalController($uibModalInstance) {
        var vm = this;

        vm.cancel = cancel;

        activate();

        function activate() { }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
