(function () {
    "use strict";

    angular
        .module("app")
        .component("postArchive", {
            templateUrl: "app/posts/post-archive.html",
            bindings: {

            },
            controller: function(postService) {
                var vm = this;
        
                activate();

                function activate() {
                    postService.getPosts(50).then(function (posts) {
                        vm.posts = posts;
                    });
                }
            }
        });
})();
