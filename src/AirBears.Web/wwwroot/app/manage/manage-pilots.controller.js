(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManagePilotsController", ManagePilotsController);

    ManagePilotsController.$inject = ["pilotService", "toast"];

    function ManagePilotsController(pilotService, toast) {
        var vm = this;

        vm.results = {};
        vm.searchCriteria = { page: 1, pageSize: 50};
        vm.markTeeShirtMailed = markTeeShirtMailed;
        vm.search = search;
        vm.pageChanged = pageChanged;

        activate();

        function activate() {
            pilotService.getPilots(vm.searchCriteria.page, vm.searchCriteria.pageSize).then(function (results) {
                vm.results = results;
            });
        }

        function search() {
            pilotService.getPilots(vm.searchCriteria.page, vm.searchCriteria.pageSize).then(function (results) {
                vm.results = results;
            });
        }

        function pageChanged() {
            search();
        }

        function markTeeShirtMailed(index, isMailed) {
            pilotService.markTeeShirtMailed(vm.results.items[index].id, isMailed).then(function (resp) {
                vm.results.items[index] = resp.data;
            },
            function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }
    }
})();
