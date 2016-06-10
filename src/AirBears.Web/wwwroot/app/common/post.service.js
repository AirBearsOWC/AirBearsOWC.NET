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
            return $http.post(urlBase, post).then(successPost, failure);
        }

        function updatePost(post) {
            return $http.put(urlBase, post).then(successPost, failure);
        }

        function getPostById(id) {
            return $http.get(urlBase + "/" + id).then(successPost, failure);
        }

        function getPostBySlug(slug) {
            return $http.get(urlBase + "/" + slug).then(successPost, failure);
        }

        function getPosts(pageSize) {
            pageSize = pageSize || 50;
            return $http.get(urlBase + "?pageSize=" + pageSize).then(successCollection, failure);
        }

        function successPost(resp) {
            return massagePost(resp.data);
        }

        function successCollection(resp) {
            var posts = [];

            angular.forEach(resp.data, function (post) {
                posts.push(massagePost(post));
            });

            return posts;
        }

        function failure(resp) {
            return $q.reject(resp);
        }

        function massagePost(post) {
            if (post.datePublished) post.datePublished = new Date(post.datePublished);
            if (post.dateUpdated) post.dateUpdated = new Date(post.dateUpdated);

            return post;
        }
    }
})();