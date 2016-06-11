(function () {
    "use strict";

    angular
        .module("app")
        .controller("PostController", PostController);

    PostController.$inject = ["$state", "$stateParams", "postService"];

    function PostController($state, $stateParams, postService) {
        var vm = this;
        
        activate();

        function activate() {
            if (!$stateParams.slug) {
                // Send back to archive.
                $state.go("root.post-archive");
            }

            postService.getPostBySlug($stateParams.slug).then(function (post) {
                vm.post = post;
            });

            postService.getPosts(5).then(function (posts) {
                vm.latestPosts = posts;
            });
        }
    }
})();
