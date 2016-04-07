(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManageCommandersController", ManageCommandersController);

    ManageCommandersController.$inject = ["pilotService", "toast"];

    function ManageCommandersController(pilotService, toast) {
        var vm = this;

        vm.pilots = [];
        vm.markTeeShirtMailed = markTeeShirtMailed;

        activate();

        function activate() {
            pilotService.getPilots().then(function (pilots) {
                vm.pilots = pilots;
            });
        }

        function markTeeShirtMailed(index, isMailed) {
            pilotService.markTeeShirtMailed(vm.pilots[index].id, isMailed).then(function (resp) {
                vm.pilots[index] = resp.data;
            },
            function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }
    }
})();
