(function () {
    "use strict";

    angular
        .module("app")
        .controller("PilotSearchController", PilotSearchController);

    PilotSearchController.$inject = ["pilotService", "toast"];

    function PilotSearchController(pilotService, toast) {
        var vm = this;

        vm.results = [];
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
        vm.search = search;

        activate();

        function activate() {
            vm.distance = vm.distances[2];
        }

        function search(isValid) {
            if (!isValid) { return; }
            
            vm.isSearching = true;

            var address = angular.isObject(vm.address) ? vm.address.formatted_address : vm.address;

            pilotService.search(address, vm.distance.value).then(function (resp) {
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
