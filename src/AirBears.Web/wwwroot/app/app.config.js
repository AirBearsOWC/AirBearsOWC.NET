(function () {
    "use strict";

    angular
        .module("app")
        .config(config);

    config.$inject = ["$provide", "$httpProvider", "uiGmapGoogleMapApiProvider"];

    function config($provide, $httpProvider, uiGmapGoogleMapApiProvider) {
        $httpProvider.interceptors.push("authInterceptor");

        uiGmapGoogleMapApiProvider.configure({
            // https://github.com/angular-ui/angular-google-maps/blob/master/src/coffee/providers/map-loader.coffee#L84
            //    key: "your api key",
            //v: "3.20", //defaults to latest 3.X anyhow
            //libraries: "weather,geometry,visualization"
            key: "AIzaSyCgM9PF1imC5ExbcVHMBvvvi0wD8wLb8lQ"
        });

        $provide.decorator("taOptions", ["taRegisterTool", "$delegate", function (taRegisterTool, taOptions) {
            taOptions.toolbar = [
              ["h2", "h3", "h4", "p", "quote"],
              ["bold", "italics", "underline", "strikeThrough", "ul", "ol", "redo", "undo", "clear"],
              ["justifyLeft", "justifyCenter", "justifyRight", "indent", "outdent"],
              ["html", "insertImage", "insertLink", "insertVideo", "wordcount", "charcount"]
            ];

            //taOptions.toolbar = [
            //  ["h1", "h2", "h3", "h4", "h5", "h6", "p", "pre", "quote"],
            //  ["bold", "italics", "underline", "strikeThrough", "ul", "ol", "redo", "undo", "clear"],
            //  ["justifyLeft", "justifyCenter", "justifyRight", "indent", "outdent"],
            //  ["html", "insertImage", "insertLink", "insertVideo", "wordcount", "charcount"]
            //];

            return taOptions;
        }]);
    }
})();
