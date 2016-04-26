(function () {
    "use strict";

    angular
        .module("app")
        .run(run);

    run.$inject = ["$rootScope", "$location", "$window"];

    function run($rootScope, $location, $window) {
        // initialise google analytics
        $window.ga("create", "UA-76762990-1", "auto");

        // track pageview on state change
        $rootScope.$on("$stateChangeSuccess", function (event) {
            if (!$window) { return; }
            $window.ga("send", "pageview", $location.path());
        });
    }
})();
