(function () {
    "use strict";

    angular
        .module("app")
        .factory("userService", userService);

    userService.$inject = ["$http"];

    function userService($http) {
        var service = {};
        var urlBase = "/api/users/";

        service.getCurrentUser = getCurrentUser;
        service.getUsers = getUsers;
        service.markTeeShirtMailed = markTeeShirtMailed;
        service.resetPassword = resetPassword;

        return service;

        function getCurrentUser() {
            return $http.get("/api/me").then(function (resp) {
                var user = resp.data;

                if (user.roles.indexOf("Admin") >= 0)
                {
                    user.isAdmin = true;
                }

                if (user.roles.indexOf("Authority") >= 0) {
                    user.isAuthority = true;
                }

                return user;
            });
        }

        function getUsers() {
            return $http.get("/api/users").then(function (resp) {
                return resp.data;
            });
        }

        function markTeeShirtMailed(userId, isMailed) {
            return $http.put("/api/pilots/" + userId + "/tee-shirt-mailed", isMailed);
        }

        function resetPassword(resetData) {
            return $http.post("/api/accounts/reset-password", resetData);
        }
    }
})();