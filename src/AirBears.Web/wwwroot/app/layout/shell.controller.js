(function () {
    "use strict";

    angular
        .module("app")
        .component("shell", {
            templateUrl: "app/layout/shell.html",
            bindings: {

            },
            controller: function ($scope, $state, registrationService, authService, userService, postService) {
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

                    postService.getPosts(3).then(function (posts) {
                        vm.latestPosts = posts;
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
        });
})();