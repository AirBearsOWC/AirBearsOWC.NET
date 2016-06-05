(function () {
    "use strict";

    angular.module("app", [
         "ui.router", 
         "ui.bootstrap", 
         "ngAnimate",
         "toaster",
         "google.places",
         "uiGmapgoogle-maps",
         "angular-loading-bar",
         "vcRecaptcha",
         "angular-storage",
         "templates"
    ]);

    angular.module("templates", []);
})();