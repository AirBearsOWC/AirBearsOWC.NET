﻿(function () {
    "use strict";

    angular
        .module("app")
        .controller("TermsController", TermsController);

    TermsController.$inject = ["$modalInstance"];

    function TermsController($modalInstance) {
        var vm = this;

        vm.agree = agree;
        vm.cancel = cancel;

        activate();

        function activate() { }

        function agree() {
            $modalInstance.close({ hasAgreed: true });
        }

        function cancel() {
            $modalInstance.dismiss("cancel");
        }
    }
})();
