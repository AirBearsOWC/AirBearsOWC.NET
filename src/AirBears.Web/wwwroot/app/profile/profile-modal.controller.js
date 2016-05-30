(function () {
    "use strict";

    angular
        .module("app")
        .controller("ProfileModalController", ProfileModalController);

    ProfileModalController.$inject = ["$uibModalInstance", "$state", "pilot"];

    function ProfileModalController($uibModalInstance, $state, pilot) {
        var vm = this;

        vm.openProfilePage = openProfilePage;
        vm.cancel = cancel;

        activate();

        function activate() {
            vm.pilot = pilot;
        }

        function openProfilePage()
        {
            $state.go("root.profile", { pilotId: vm.pilot.id, pilot: vm.pilot });
            $uibModalInstance.dismiss("cancel");
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }
})();
