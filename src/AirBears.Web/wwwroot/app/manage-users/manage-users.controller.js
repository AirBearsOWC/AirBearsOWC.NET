(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManageUsersController", ManageUsersController);

    ManageUsersController.$inject = ["userService", "toast"];

    function ManageUsersController(userService, toast) {
        var vm = this;

        vm.users = [];
        vm.markTeeShirtMailed = markTeeShirtMailed;

        activate();

        function activate() {
            userService.getUsers().then(function (users) {
                vm.users = users;
            });
        }

        function markTeeShirtMailed(index, isMailed) {
            userService.markTeeShirtMailed(vm.users[index].id, isMailed).then(function (resp) {
                vm.users[index] = resp.data;
            },
            function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }
    }
})();
