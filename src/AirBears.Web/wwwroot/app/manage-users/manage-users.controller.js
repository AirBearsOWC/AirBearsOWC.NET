(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManageUsersController", ManageUsersController);

    ManageUsersController.$inject = ["userService"];

    function ManageUsersController(userService) {
        var vm = this;

        vm.users = [];

        activate();

        function activate() {
            userService.getUsers().then(function (users) {
                vm.users = users;
            });
        }
    }
})();
