(function () {
    "use strict";

    angular
        .module("app")
        .controller("ProfileController", ProfileController);

    ProfileController.$inject = ["$state", "pilotService", "userService", "toast"];

    function ProfileController($state, pilotService, userService, toast) {
        var vm = this;

        activate();

        function activate() {
            if ($state.params.pilot) {
                vm.pilot = $state.params.pilot;
            }
            else {
                pilotService.getPilot($state.params.pilotId).then(function (pilot) {
                    vm.pilot = pilot;
                },
                function (resp) {
                    if (resp.status !== 401) {
                        toast.pop("error", "Search Error", "", resp.data);
                    }
                });
            }

            userService.getCurrentUser().then(function (user) {
                vm.user = user;
            });
        }
    }
})();
