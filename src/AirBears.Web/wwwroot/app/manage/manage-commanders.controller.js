(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManageCommandersController", ManageCommandersController);

    ManageCommandersController.$inject = ["authorityService", "toast"];

    function ManageCommandersController(authorityService, toast) {
        var vm = this;

        vm.results = {};
        vm.sortOptions = [{ name: "First Name", value: "firstName" }, { name: "Last Name", value: "lastName" }, { name: "Registration Date", value: "dateRegistered" }];
        vm.searchCriteria = { page: 1, pageSize: 50, sortBy: vm.sortOptions[1].value, ascending: true, onlyUnapproved: false };

        vm.markApproved = markApproved;
        vm.search = search;
        vm.toggleSortOrder = toggleSortOrder;
        vm.toggleFilterUnapproved = toggleFilterUnapproved;
        vm.pageChanged = pageChanged;

        activate();

        function activate() {
            search();
        }

        function pageChanged() {
            search();
        }

        function search() {
            authorityService.getUsers(
                vm.searchCriteria.page,
                vm.searchCriteria.pageSize,
                vm.searchCriteria.name,
                vm.searchCriteria.onlyUnapproved,
                vm.searchCriteria.sortBy,
                vm.searchCriteria.ascending).then(function (results) {
                    vm.results = results;
                });
        }

        function toggleSortOrder() {
            vm.searchCriteria.ascending = !vm.searchCriteria.ascending;
            search();
        }

        function toggleFilterUnapproved() {
            vm.searchCriteria.onlyUnapproved = !vm.searchCriteria.onlyUnapproved;
            search();
        }

        function markApproved(index, isApproved) {
            authorityService.markApproved(vm.results.items[index].id, isApproved).then(function (resp) {
                vm.results.items[index] = resp.data;
            },
            function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }
    }
})();
