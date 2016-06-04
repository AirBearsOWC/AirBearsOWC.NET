(function () {
    "use strict";

    angular
        .module("app")
        .filter("ceil", ceil);

    ceil.$inject = [];

    function ceil() {
        return function (input) {
            return Math.ceil(input);
        };
    }
})();