(function () {
    "use strict";

    angular
        .module("app")
        .directive("abCompareTo", abCompareTo);

    abCompareTo.$inject = [];

    function abCompareTo() {
        var directive = {
            restrict: "A",
            require: "ngModel",
            scope: {
                otherModelValue: "=abCompareTo"
            },
            link: link
        };

        return directive;

        function link(scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue === scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    }
})();