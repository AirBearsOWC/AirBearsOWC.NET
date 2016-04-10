(function () {
    "use strict";

    angular
        .module("app")
        .controller("PilotWindowController", PilotWindowController);

    PilotWindowController.$inject = ["$uibModal"];

    function PilotWindowController($uibModal) {
        var vm = this;

        vm.openProfileModal = openProfileModal;

        activate();

        function activate() {

        }

        function openProfileModal(pilot) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/profile/profile-modal.html",
                controller: "ProfileModalController as vm",
                resolve: {
                    pilot: function () {
                        return pilot;
                    }
                },
                size: "md"
            });
        }
    }
})();