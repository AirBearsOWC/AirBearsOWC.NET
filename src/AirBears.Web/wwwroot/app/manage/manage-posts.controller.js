(function () {
    "use strict";

    angular
        .module("app")
        .controller("ManagePostsController", ManagePostsController);

    ManagePostsController.$inject = ["$state", "postService", "$uibModal", "toast"];

    function ManagePostsController($state, postService, $uibModal, toast) {
        var vm = this;

        vm.posts = [];

        vm.createPost = createPost;
        vm.editPost = editPost;
        vm.viewPost = viewPost;
        vm.deletePost = deletePost;

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

        function deletePost(post) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/common/confirm-modal.html",
                controller: "ConfirmModalController as vm",
                resolve: {
                    options: function () {
                        return {
                            title: "Confirm Deletion",
                            content: "Are you sure you want to delete this post? This action cannot be undone.",
                            confirmBtnText: "Yes, Delete the Post",
                            confirmBtnClass: "btn-danger"
                        };
                    }
                },
                size: "sm"
            });

            modalInstance.result.then(function (confirmed) {
                if(confirmed) {
                    postService.deletePost(post.id).then(function () {
                        var index = vm.posts.indexOf(post);
                        vm.posts.splice(index, 1);

                        toast.pop("success", "Success!", post.title + " was deleted.");
                    }, function () {
                        toast.pop("error", "Delete Failed", "", resp.data);
                    });
                }
            });
        }
    }
})();
