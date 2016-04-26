(function () {
    "use strict";

    angular
        .module("app")
        .controller("ShellController", ShellController);

    ShellController.$inject = ["$scope", "$state", "registrationService", "authService", "userService"]; 

    function ShellController($scope, $state, registrationService, authService, userService) {
        var vm = this;

        vm.user = null;
        vm.isActive = isActive;
        vm.openRegistationOptions = registrationService.openRegistationOptions;
        vm.openLogin = authService.openLogin;
        vm.logout = logout;

        activate();

        function activate() {
            if (authService.getAuthToken()) {
                getCurrentUser();
            }

            authService.onLogin($scope, function () {
                getCurrentUser();
            });

            authService.onUnauthenticated($scope, function () {
                vm.user = null;
            });
        }

        function isActive(routeSubstring) {
            return $state.current.name.indexOf(routeSubstring) >= 0;
        }

        function logout() {
            authService.logout();
            vm.user = null;
            if (!$state.is("root.home")) { $state.go("root.home"); }
        }

        function getCurrentUser() {
            return userService.getCurrentUser().then(function (user) {
                vm.user = user;
                return user;
            });
        }
    }
})();
