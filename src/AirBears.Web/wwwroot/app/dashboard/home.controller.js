(function () {
    "use strict";

    angular
        .module("app")
        .component("home", {
            templateUrl: "app/dashboard/home.html",
            bindings: {

            },
            controller: function (postService) {
                var vm = this;

                activate();

                function activate() {
                    postService.getPosts(3).then(function (posts) {
                        vm.latestPosts = posts;
                    });
                }
            }
        });
})();
