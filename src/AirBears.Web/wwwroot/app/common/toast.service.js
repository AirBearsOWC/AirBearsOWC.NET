(function () {
    "use strict";

    angular
        .module("app")
        .factory("toast", toast);

    toast.$inject = ["toaster"];

    function toast(toaster) {
        var service = {};

        service.pop = pop;

        return service;

        function pop(type, title, msg, data) {
            var toast = {
                type: type,
                title: title,
                body: msg + " " + flattenMessage(data),
                bodyOutputType: "trustedHtml",
                showCloseButton: true,
                timeout: 5000
            };

            if (type === "error") {
                toast.timeout = 9000;
            }

            toaster.pop(toast);
        }

        function flattenMessage(data) {
            var msg = "";

            if (!data) { return msg; }
            if (!angular.isObject(data)) { return data; }

            if (Object.keys(data).length === 1) {
                // There is only one error, so don't create UL.
                msg += data[Object.keys(data)[0]]
            }
            else {
                // There are multiple errors, so put them in a UL.
                msg += "<ul style='margin: 0;'>";
                for (var key in data) {
                    for (var i = 0; i < data[key].length; i++) {
                        msg += "<li>" + data[key][i] + "</li>";
                    }
                }
                msg += "</ul>"
            }

            if (msg.length > 199) { msg = msg.substring(0, 200) + "..."; }

            return msg;
        }
    }
})();