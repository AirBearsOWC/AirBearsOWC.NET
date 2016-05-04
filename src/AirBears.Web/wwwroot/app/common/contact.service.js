(function () {
    "use strict";

    angular
        .module("app")
        .factory("contactService", contactService);

    contactService.$inject = ["$http", "$q"];

    function contactService($http, $q) {
        var service = {};
        var urlBase = "/api/messages";

        service.send = send;

        return service;

        function send(message) {
            return $http.post(urlBase, message).then(function (resp) {
                return resp.data;
            }, function (resp) {
                return $q.reject(resp);
            });
        }
    }
})();