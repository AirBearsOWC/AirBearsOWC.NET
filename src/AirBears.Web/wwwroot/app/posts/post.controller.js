(function () {
    "use strict";

    angular
        .module("app")
        .component("post", {
            templateUrl: "app/posts/post.html",
            bindings: {

            },
            controller: function ($state, $stateParams, postService) {
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
                        var indexOfCurrentPost = -1;

                        // Look for the current post in the list of posts and remove it if it's found.
                        for (var i = 0; i < posts.length; i++) {
                            if (posts[i].slug === $stateParams.slug) {
                                indexOfCurrentPost = i;
                                break;
                            }
                        }

                        if (indexOfCurrentPost >= 0) {
                            posts.splice(indexOfCurrentPost, 1);
                        }

                        vm.latestPosts = posts;
                    });
                }
            }
        });
})();
