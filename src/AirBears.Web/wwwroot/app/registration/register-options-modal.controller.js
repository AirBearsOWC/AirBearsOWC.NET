(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterOptionsModalController", RegisterOptionsModalController);

    RegisterOptionsModalController.$inject = ["$uibModalInstance", "$state"];

    function RegisterOptionsModalController($uibModalInstance, $state) {
        var vm = this;

        vm.navigateTo = navigateTo;
        vm.cancel = cancel;

        activate();

        function activate() { }

        function navigateTo(state) {
            $state.go(state);
            $uibModalInstance.dismiss("cancel");
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
