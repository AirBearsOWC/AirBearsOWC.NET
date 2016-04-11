(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManagePilotsController", ManagePilotsController);

    ManagePilotsController.$inject = ["pilotService", "toast"];

    function ManagePilotsController(pilotService, toast) {
        var vm = this;

        vm.results = {};       
        vm.sortOptions = [{ name: "First Name", value: "firstName" }, { name: "Last Name", value: "lastName" }, { name: "Registration Date", value: "dateRegistered" }];
        vm.searchCriteria = { page: 1, pageSize: 50, sortBy: vm.sortOptions[1].value, ascending: true, onlyShirtsNotSent: false };

        vm.markTeeShirtMailed = markTeeShirtMailed;
        vm.search = search;
        vm.toggleSortOrder = toggleSortOrder;
        vm.toggleFilterShirts = toggleFilterShirts;
        vm.pageChanged = pageChanged;

        activate();

        function activate() {
            search();
        }

        function pageChanged() {
            search();
        }

        function search() {
            pilotService.getPilots(
                vm.searchCriteria.page,
                vm.searchCriteria.pageSize,
                vm.searchCriteria.name,
                vm.searchCriteria.onlyShirtsNotSent,
                vm.searchCriteria.sortBy,
                vm.searchCriteria.ascending).then(function (results) {
                vm.results = results;
            });
        }

        function toggleSortOrder() {
            vm.searchCriteria.ascending = !vm.searchCriteria.ascending;
            search();
        }

        function toggleFilterShirts() {
            vm.searchCriteria.onlyShirtsNotSent = !vm.searchCriteria.onlyShirtsNotSent;
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
