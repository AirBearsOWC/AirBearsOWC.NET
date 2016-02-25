(function () {
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
        vm.isSearching = false;

        vm.toggleMarkerWindow = toggleMarkerWindow;
        vm.selectPilot = selectPilot;
        vm.search = search;

        activate();

        function activate() {
            // Set default distance to 10 miles.
            vm.distance = vm.distances[2];
        }

        function selectPilot(pilot) {
            pilot.show = !pilot.show;
        }

        function toggleMarkerWindow(marker, eventName, model) {
            selectPilot(model);
        };

        function search(isValid) {
            if (!isValid) { return; }
            
            vm.isSearching = true;

            var lat = null;
            var lng = null;
            var address = vm.address;

            //var address = angular.isObject(vm.address) ? vm.address.formatted_address : vm.address;

            if (angular.isObject(vm.address)) {
                lat = vm.address.geometry.location.lat();
                lng = vm.address.geometry.location.lng();
                address = null;

                var radius = (vm.distance.value / 3963.1676) * 6378100;
                var scale = radius / 500;
                var zoomLevel = Math.round(16.5 - Math.log(scale) / Math.log(2)) - 1;

                vm.map = { center: { latitude: lat, longitude: lng }, zoom: zoomLevel };
                vm.circle = {
                    center: {
                        latitude: lat,
                        longitude: lng
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

            pilotService.search(address, vm.distance.value, lat, lng).then(function (resp) {
                //angular.forEach(resp.data, function (pilot) {
                //    pilot.options = {
                //        icon: {
                //            path: google.maps.SymbolPath.CIRCLE,
                //            scale: 4
                //        }
                //    };
                //});
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