(function () {
    "use strict";

    angular
        .module("app")
        .controller("PilotSearchController", PilotSearchController);

    PilotSearchController.$inject = ["$scope", "toast", "pilotService", "authService", "userService", "registrationService"];

    function PilotSearchController($scope, toast, pilotService, authService, userService, registrationService) {
        var vm = this;

        vm.user = null;
        vm.results = null;
        vm.distances = [
            { value: 1, name: "1 mile" },
            { value: 5, name: "5 miles" },
            { value: 10, name: "10 miles" },
            { value: 25, name: "25 miles" },
            { value: 50, name: "50 miles" },
            { value: 100, name: "100 miles" },
            { value: 250, name: "250 miles" },
            { value: 500, name: "500 miles" },
            { value: 1000, name: "1000 miles" }
        ];
        vm.isSearching = false;

        vm.toggleMarkerWindow = toggleMarkerWindow;
        vm.selectPilot = selectPilot;
        vm.search = search;
        vm.openRegistationOptions = registrationService.openRegistationOptions;
        vm.openLogin = authService.openLogin;

        activate();

        function activate() {
            if (authService.getAuthToken()) {
                getCurrentUser();
            }
            else { vm.user = null; }

            // Set default distance to 10 miles.
            vm.distance = vm.distances[2];

            authService.onLogin($scope, function () {
                getCurrentUser();
            });
        }

        function selectPilot(pilot) {
            pilot.show = !pilot.show;
        }

        function toggleMarkerWindow(marker, eventName, model) {
            selectPilot(model);
        };

        function search(isValid) {
            if (!isValid) { return; }

            var searchCriteria = { distance: vm.distance.value, address: vm.address, latitude: null, longitude: null, page: 1, pageSize: 10 };
            
            vm.isSearching = true;

            //var address = angular.isObject(vm.address) ? vm.address.formatted_address : vm.address;

            if (angular.isObject(vm.address)) {
                searchCriteria.latitude = vm.address.geometry.location.lat();
                searchCriteria.longitude = vm.address.geometry.location.lng();
                searchCriteria.address = null;

                var radius = (vm.distance.value / 3963.1676) * 6378100;
                var scale = radius / 500;
                var zoomLevel = Math.round(16.5 - Math.log(scale) / Math.log(2)) - 1;

                vm.map = { center: { latitude: searchCriteria.latitude, longitude: searchCriteria.longitude }, zoom: zoomLevel };
                vm.circle = {
                    center: {
                        latitude: searchCriteria.latitude,
                        longitude: searchCriteria.longitude
                    },
                    radius: radius,
                    stroke: {
                        color: "#08B21F",
                        weight: 1,
                        opacity: 0.5
                    },
                    fill: {
                        color: "#08B21F",
                        opacity: 0.2
                    }
                };
            }           

            pilotService.search(searchCriteria).then(function (data) {
                angular.forEach(data.items, function (item) {
                    item.windowTemplate = "app/pilot-search/pilot-window.html";
                });
                vm.results = data;
                vm.isSearching = false;
            }, 
            function (resp) {
                vm.isSearching = false;
                if (resp.status !== 401) {
                    toast.pop("error", "Search Error", "", resp.data);
                }
            });
        }

        function getCurrentUser() {
            return userService.getCurrentUser().then(function (user) {
                vm.user = user;
            }, function (error) {
                vm.user = null;
            });
        }
    }
})();