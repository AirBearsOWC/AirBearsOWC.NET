(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManagePostsController", ManagePostsController);

    ManagePostsController.$inject = ["$state", "postsService", "toast"];

    function ManagePostsController($state, postsService, toast) {
        var vm = this;

        vm.posts = [];

        vm.createPost = createPost;
        vm.editPost = editPost;
        vm.viewPost = viewPost;

        activate();

        function activate() {
            postsService.getPosts().then(function (posts) {
                vm.posts = posts;
            });
        }
      
        function createPost() {

        }

        function editPost() {

        }

        function viewPost() {

        }
    }
})();
