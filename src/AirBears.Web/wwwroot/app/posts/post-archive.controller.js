(function () {
    "use strict";

    angular
        .module("app")
        .controller("PostArchiveController", PostArchiveController);

    PostArchiveController.$inject = ["postService"];

    function PostArchiveController(postService) {
        var vm = this;
        
        activate();

        function activate() {
            postService.getPosts(50).then(function (posts) {
                vm.posts = posts;
            });
        }
    }
})();
