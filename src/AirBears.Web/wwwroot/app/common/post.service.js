(function () {
    "use strict";

    angular
        .module("app")
        .factory("postService", postService);

    postService.$inject = ["$http", "$q"];

    function postService($http, $q) {
        var service = {};
        var urlBase = "/api/posts";

        service.createPost = createPost;
        service.updatePost = updatePost;
        service.getPostById = getPostById;
        service.getPostBySlug = getPostBySlug;
        service.getPosts = getPosts;

        return service;

        function createPost(post) {
            return $http.post(urlBase, post).then(success, failure);
        }

        function updatePost(post) {
            return $http.put(urlBase, post).then(success, failure);
        }

        function getPostById(id) {
            return $http.get(urlBase + "/" + id).then(success, failure);
        }

        function getPostBySlug(slug) {
            return $http.get(urlBase + "/" + slug).then(success, failure);
        }

        function getPosts() {
            return $http.get(urlBase).then(success, failure);
        }

        function success(resp) {
            return resp.data;
        }

        function failure(resp) {
            return $q.reject(resp);
        }
    }
})();