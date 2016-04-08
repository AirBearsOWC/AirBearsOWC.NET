(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManageCommandersController", ManageCommandersController);

    ManageCommandersController.$inject = ["authorityService", "toast"];

    function ManageCommandersController(authorityService, toast) {
        var vm = this;

        vm.users = [];
        vm.markApproved = markApproved;

        activate();

        function activate() {
            authorityService.getUsers().then(function (users) {
                vm.users = users;
            });
        }

        function markApproved(index, isApproved) {
            authorityService.markApproved(vm.users[index].id, isApproved).then(function (resp) {
                vm.users[index] = resp.data;
            },
            function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }
    }
})();
