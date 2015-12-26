(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterOptionsModalController", RegisterOptionsModalController);

    RegisterOptionsModalController.$inject = ["$uibModalInstance"];

    function RegisterOptionsModalController($uibModalInstance) {
        var vm = this;

        vm.cancel = cancel;

        activate();

        function activate() { }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
