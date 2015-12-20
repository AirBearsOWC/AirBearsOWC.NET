(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterPilotController", RegisterPilotController);

    RegisterPilotController.$inject = [];

    function RegisterPilotController() {
        var vm = this;

        activate();

        function activate() { }
    }
})();
