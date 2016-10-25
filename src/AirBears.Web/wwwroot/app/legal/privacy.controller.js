(function () {
    "use strict";

    angular
        .module("app")
        .component("privacy", {
            templateUrl: "app/legal/privacy.html",
            bindings: {

            },
            controller: function () {
                var vm = this;

                activate();

                function activate() {

                }
            }
        });
})();
