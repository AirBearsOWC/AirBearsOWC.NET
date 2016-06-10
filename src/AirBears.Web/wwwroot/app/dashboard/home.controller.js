(function () {
    "use strict";

    angular
        .module("app")
        .controller("HomeController", HomeController);

    HomeController.$inject = ["postService"]; 

    function HomeController(postService) {
        var vm = this;

        activate();

        function activate() {
            postService.getPosts(3).then(function (posts) {
                vm.latestPosts = posts;
            });
        }
    }
})();
