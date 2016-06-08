(function () {
    "use strict";

    angular
        .module("app")
        .controller("EditPostController", EditPostController);

    EditPostController.$inject = ["$stateParams", "$state", "postService", "toast"];

    function EditPostController($stateParams, $state, postService, toast) {
        var vm = this;
        var isNewPost = true;

        vm.save = save;

        activate();

        function activate() {
            if ($stateParams.postId && $stateParams.postId != "new") {
                isNewPost = false;
                postService.getPostById($stateParams.postId).then(function (post) {
                    vm.post = post;
                });
            }
        }
      
        function save(isValid) {
            if (!isValid) { return; }

            if (isNewPost) {
                postService.createPost(vm.post).then(function (post) {
                    toast.pop("success", "Success!", "Your post was created.");
                    $state.go("root.edit-post", { postId: post.id });
                }, function (resp) {
                    toast.pop("error", "Create Post Failed", "", resp.data);
                });
            }
            else {
                postService.updatePost(vm.post).then(function () {
                    toast.pop("success", "Success!", "Your post was updated.");
                }, function (resp) {
                    toast.pop("error", "Update Post Failed", "", resp.data);
                });
            }
        }
    }
})();
