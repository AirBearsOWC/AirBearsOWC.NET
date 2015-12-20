(function () {
    "use strict";

    angular
        .module("app")
        .controller("RegisterAuthorityController", RegisterAuthorityController);

    RegisterAuthorityController.$inject = [];

    function RegisterAuthorityController() {
        var vm = this;

        activate();

        function activate() { }
    }
})();
