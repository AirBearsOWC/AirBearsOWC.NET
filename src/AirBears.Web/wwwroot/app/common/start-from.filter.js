(function () {
    "use strict";

    angular
        .module("app")
        .filter("startFrom", startFrom);

    startFrom.$inject = [];

    function startFrom() {
        return function (input, start) {
            if (!input) return;

            start = +start; //parse to int
            return input.slice(start);
        }
    }
})();