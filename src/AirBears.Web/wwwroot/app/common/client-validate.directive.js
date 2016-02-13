(function () {
    "use strict";

    /**
    * @desc
    * Simplifies client-side validation when applied to form-group elements by automatically
    * managing the "has-error" class logic based on field validity. Any elements with
    * the "help-block validation" classes will automatically shown when the input is invalid.
    *
    * @example <div class="form-group" ab-client-validate>
    *               <input id="firstName" name="firstName" type="text" class="form-control" required ng-model="vm.firstName" />
    *               <div class="help-block validation" ng-show="userForm.firstName.$error.required">First Name is required.</div>
    *          </div>
    */
    angular
        .module("app")
        .directive("abClientValidate", abClientValidate);

    abClientValidate.$inject = [];

    function abClientValidate() {
        var directive = {
            restrict: "A",
            require: "^form",
            link: link
        };

        return directive;

        function link(scope, ele, attrs, c) {
            // find the input/select element which has the "name" attribute
            var inputEle = ele[0].querySelector("[name]");

            if (!inputEle) return;

            scope.$watch(c.$name + ".$submitted", function (submitted) {
                ele.toggleClass("has-error", submitted && c[inputNgEle.attr("name")].$invalid);
            });

            // convert the native input element to an angular element
            var inputNgEle = angular.element(inputEle);

            // only apply the has-error class to the form-group after the user leaves the input (if invalid)
            inputNgEle.bind("blur", function () {
                ele.toggleClass("has-error", c[inputNgEle.attr("name")].$invalid && c[inputNgEle.attr("name")].$touched);
            })
        }
    }
})();