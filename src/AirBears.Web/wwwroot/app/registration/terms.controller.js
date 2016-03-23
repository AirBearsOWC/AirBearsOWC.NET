(function () {
    "use strict";

    angular
        .module("app")
        .controller("TermsController", TermsController);

    TermsController.$inject = ["$uibModalInstance"];

    function TermsController($uibModalInstance) {
        var vm = this;

        vm.agree = agree;
        vm.cancel = cancel;

        activate();

        function activate() { }

        function agree() {
            $uibModalInstance.close({ hasAgreed: true });
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
