(function () {
    "use strict";

    angular
        .module("app")
        .config(config);

    config.$inject = ["$httpProvider", "uiGmapGoogleMapApiProvider"];

    function config($httpProvider, uiGmapGoogleMapApiProvider) {
        $httpProvider.interceptors.push("authInterceptor");

        uiGmapGoogleMapApiProvider.configure({
            // https://github.com/angular-ui/angular-google-maps/blob/master/src/coffee/providers/map-loader.coffee#L84
            //    key: "your api key",
            //v: "3.20", //defaults to latest 3.X anyhow
            //libraries: "weather,geometry,visualization"
            key: "AIzaSyCgM9PF1imC5ExbcVHMBvvvi0wD8wLb8lQ"
        });
    }
})();
