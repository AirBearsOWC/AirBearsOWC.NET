(function () {
    "use strict";

    angular
        .module("app")
        .component("contact", {
            templateUrl: "app/contact/contact.html",
            bindings: {

            },
            controller: function ($state, toast, contactService) {
                var vm = this;

                vm.messageSent = false;
                vm.send = send;

                activate();

                function activate() {

                }

                function send(isValid) {
                    if (!isValid) { return; }

                    vm.isSubmitting = true;

                    contactService.send(vm.form).then(function () {
                        toast.pop("success", "Message Sent", "Your message was successfully sent!");
                        vm.isSubmitting = false;
                        vm.messageSent = true;
                        vm.form = {};
                    }, function (resp) {
                        toast.pop("error", "Message Failed", "", resp.data);
                        vm.isSubmitting = false;
                    });
                }
            }
        });
})();
