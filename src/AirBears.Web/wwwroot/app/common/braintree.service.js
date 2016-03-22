(function () {
    "use strict";

    angular
        .module("app")
        .factory("braintreeService", braintreeService);

    braintreeService.$inject = ["$http", "$timeout"];

    function braintreeService($http, $timeout) {
        var service = {};
        var checkout;

        service.getClientToken = getClientToken;
        service.setupHeadlessPaypal = setupHeadlessPaypal;
        service.startAuthFlow = startAuthFlow;

        return service;

        function getClientToken() {
            return $http.get("/api/payment-token").then(function(resp){
                return resp.data;
            });
        }

        function startAuthFlow() {
            if (!checkout) { throw new Error("You must setup auth flow before starting auth flow."); }

            checkout.paypal.initAuthFlow();
        }

        function setupHeadlessPaypal(amount, cb) {
            getClientToken().then(function (token) {
                braintree.setup(token, "custom", {
                    onReady: function (integration) {
                        checkout = integration;
                    },
                    onPaymentMethodReceived: function (data) {
                        $timeout(function () { cb(data); }, 0);
                    },
                    paypal: {
                        singleUse: true,
                        amount: amount,
                        currency: "USD",
                        locale: "en_us",
                        headless: true
                    }
                });
            });
        }
    }
})();