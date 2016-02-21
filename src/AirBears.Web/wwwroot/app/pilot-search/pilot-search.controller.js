﻿(function () {
    "use strict";

    angular
        .module("app")
        .controller("PilotSearchController", PilotSearchController);

    PilotSearchController.$inject = ["pilotService", "toast"];

    function PilotSearchController(pilotService, toast) {
        var vm = this;

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
        vm.markerOptions = {
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 4
            }
        };
        vm.windowOptions = {
            visible: false
        };
        vm.isSearching = false;

        vm.toggleMarkerWindow = toggleMarkerWindow;
        vm.closeMarkerWindow = closeMarkerWindow;
        vm.search = search;

        activate();

        function activate() {
            vm.distance = vm.distances[2];
        }

        function toggleMarkerWindow() {
            vm.windowOptions.visible = !vm.windowOptions.visible;
        };

        function closeMarkerWindow() {
            vm.windowOptions.visible = false;
        };

        function search(isValid) {
            if (!isValid) { return; }
            
            vm.isSearching = true;

            var lat = null;
            var lng = null;
            var address = vm.address;

            //var address = angular.isObject(vm.address) ? vm.address.formatted_address : vm.address;

            if (angular.isObject(vm.address)){
                lat = vm.address.geometry.location.lat();
                lng = vm.address.geometry.location.lng();
                address = null;

                vm.map = { center: { latitude: lat, longitude: lng }, zoom: 8 };
            }

            pilotService.search(address, vm.distance.value, lat, lng).then(function (resp) {
                vm.results = resp.data;
                vm.isSearching = false;
            }, 
            function (resp) {
                vm.isSearching = false;
                toast.pop("error", "Search Error", "", resp.data);
            });
        }
    }
})();