(function () {
    "use strict";

    angular
        .module("app")
        .controller("ConfirmModalController", ConfirmModalController);

    ConfirmModalController.$inject = ["$uibModalInstance", "options"];

    function ConfirmModalController($uibModalInstance, options) {
        var vm = this;

        vm.options = {
            title: "Confirm",
            content: "Are you sure?",
            confirmBtnText: "Ok",
            confirmBtnClass: "btn-primary"
        };

        vm.ok = ok;
        vm.cancel = cancel;

        activate();

        function activate() {
            if (!options) { return; }

            vm.options.title = options.title || vm.options.title;
            vm.options.content = options.content || vm.options.content;
            vm.options.confirmBtnText = options.confirmBtnText || vm.options.confirmBtnText;
            vm.options.confirmBtnClass = options.confirmBtnClass || vm.options.confirmBtnClass;
        }

        function ok() {
            $uibModalInstance.close(true);
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
