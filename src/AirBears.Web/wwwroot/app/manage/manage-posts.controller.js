(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManagePostsController", ManagePostsController);

    ManagePostsController.$inject = ["$state", "postService", "toast"];

    function ManagePostsController($state, postService, toast) {
        var vm = this;

        vm.posts = [];

        vm.createPost = createPost;
        vm.editPost = editPost;
        vm.viewPost = viewPost;

        activate();

        function activate() {
            postService.getPosts(50, true).then(function (posts) {
                vm.posts = posts;
            });
        }
      
        function createPost() {
            $state.go("root.edit-post", { postId: "new" });
        }

        function editPost(postId) {
            $state.go("root.edit-post", { postId: postId });
        }

        function viewPost() {

        }
    }
})();
