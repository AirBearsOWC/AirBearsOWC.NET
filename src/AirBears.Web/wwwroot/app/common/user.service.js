(function () {
    "use strict";

    angular
        .module("app")
        .factory("userService", userService);

    userService.$inject = ["$http", "$q"];

    function userService($http, $q) {
        var service = {};
        var urlBase = "/api/users/";

        service.getCurrentUser = getCurrentUser;
        service.getUsers = getUsers;
        service.markTeeShirtMailed = markTeeShirtMailed;
        service.recoverPassword = recoverPassword;
        service.resetPassword = resetPassword;
        service.changePassword = changePassword;

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
            }, function (resp) {
                return $q.reject(resp);
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

        function recoverPassword(email) {
            return $http.post("/api/accounts/forgot-password", { email: email });
        }

        function resetPassword(resetData) {
            return $http.post("/api/accounts/reset-password", resetData);
        }

        function changePassword(passwordData) {
            return $http.post("/api/me/password", passwordData);
        }
    }
})();