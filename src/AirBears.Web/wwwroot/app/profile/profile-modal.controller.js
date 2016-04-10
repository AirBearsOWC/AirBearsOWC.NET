(function () {
    "use strict";

    angular
        .module("app")
        .controller("ProfileModalController", ProfileModalController);

    ProfileModalController.$inject = ["$uibModalInstance", "pilot"];

    function ProfileModalController($uibModalInstance, pilot) {
        var vm = this;

        vm.cancel = cancel;

        activate();

        function activate() {
            vm.pilot = pilot;
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
