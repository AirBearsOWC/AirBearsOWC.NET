(function () {
    "use strict";

    angular
        .module("app")
        .controller("EditProfileController", EditProfileController);

    EditProfileController.$inject = ["$uibModal", "resourceService", "userService", "pilotService", "authService", "toast"];

    function EditProfileController($uibModal, resourceService, userService, pilotService, authService, toast) {
        var vm = this;

        vm.states = [];
        vm.teeShirtSizes = [];
        vm.pilot = {};

        vm.openChangePasswordModal = openChangePasswordModal;
        vm.openEditBioModal = openEditBioModal;
        vm.openEditProfileModal = openEditProfileModal;
        vm.toggleAllowsPilotSearch = updatePrivacySettings;
        vm.toggleSubscribesToUpdates = updatePrivacySettings;
        vm.toggleNightVision = updatePlatformCapabilities;
        vm.toggleThermal = updatePlatformCapabilities;       
        vm.chooseFlightTime = updatePlatformCapabilities;
        vm.choosePayload = updatePlatformCapabilities;
        vm.toggleFemaIcsCertified = updateCertifications;
        vm.toggleHamRadioLicensed = updateCertifications;

        activate();

        function activate() {
            getCurrentUser();

            resourceService.getStates().then(function (states) {
                vm.states = states;
            });

            resourceService.getTeeShirtSizes().then(function (sizes) {
                vm.teeShirtSizes = sizes;
            });

            resourceService.getPayloads().then(function (payloads) {
                vm.payloads = payloads;
            });
            resourceService.getFlightTimes().then(function (flightTimes) {
                vm.flightTimes = flightTimes;
            });
        }

        function getCurrentUser() {
            userService.getCurrentUser().then(function (user) {
                vm.pilot = user;
            }, function () {
                authService.openLogin(function () {
                    getCurrentUser();
                });
            });
        }

        function updatePrivacySettings() {
            pilotService.updateProfile(vm.pilot).then(function () {
                toast.pop("success", "Update Success!", "Your privacy settings have been updated.");
            }, function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }

        function updatePlatformCapabilities() {
            pilotService.updateProfile(vm.pilot).then(function () {
                toast.pop("success", "Update Success!", "Your platform capabilities have been updated.");
            }, function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }

        function updateCertifications() {
            pilotService.updateProfile(vm.pilot).then(function () {
                toast.pop("success", "Update Success!", "Your certifications have been updated.");
            }, function (resp) {
                toast.pop("error", "Update Failed", "", resp.data);
            });
        }

        function openEditBioModal() {
            var oldBio = vm.pilot.bio;
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/profile/edit-bio-modal.html",
                controller: "EditBioModalController as vm",
                resolve: {
                    pilot: function () {
                        return vm.pilot;
                    }
                },
                size: "md"
            });

            modalInstance.result.then(function (result) {
                vm.pilot.bio = result;
            }, function () {
                // put the bio back to what it was in case they cancel the modal window.
                vm.pilot.bio = oldBio;
            });
        }

        function openEditProfileModal() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/profile/edit-bio-modal.html",
                controller: "EditBioModalController as vm",
                resolve: {
                    pilot: function () {
                        return vm.pilot;
                    }
                },
                size: "md"
            });

            modalInstance.result.then(function (result) {
                vm.pilot.bio = result;
            });
        }

        function openChangePasswordModal() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: "app/profile/change-password-modal.html",
                controller: "ChangePasswordModalController as vm",
                size: "md"
            });
        }
    }
})();
