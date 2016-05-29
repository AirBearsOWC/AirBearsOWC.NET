using AirBears.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AirBears.Web.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime DateRegistered { get; set; }
    }

    public class PilotViewModel : UserViewModel
    {
        [MaxLength(100)]
        public string Street1 { get; set; }

        [MaxLength(100)]
        public string Street2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public State State { get; set; }

        [MaxLength(10)]
        public string Zip { get; set; }

        /// <summary>
        /// Line 1 of international address
        /// </summary>
        [MaxLength(100)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Line 2 of international address
        /// </summary>
        [MaxLength(100)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Line 3 of international address
        /// </summary>
        [MaxLength(100)]
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Line 4 of international address
        /// </summary>
        [MaxLength(100)]
        public string AddressLine4 { get; set; }

        public bool HasInternationalAddress { get; set; }

        [MaxLength(200)]
        public string GeocodeAddress { get; set; }

        public TeeShirtSize TeeShirtSize { get; set; }

        public DateTime? TeeShirtMailedDate { get; set; }

        public bool AllowsPilotSearch { get; set; }

        public bool SubscribesToUpdates { get; set; }

        public bool NightVisionCapable { get; set; }

        public bool ThermalVisionCapable { get; set; }

        public bool FemaIcsCertified { get; set; }

        public bool HamRadioLicensed { get; set; }

        public Payload Payload { get; set; }

        public FlightTime FlightTime { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }
    }

    public class PilotSearchResultViewModel : PilotViewModel
    {
        /// <summary>
        /// Distance in miles from the search address.
        /// </summary>
        public double Distance { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }
    }

    public class IdentityViewModel : UserViewModel
    {
        public IEnumerable<string> Roles { get; set; }
    }

    public class IdentityPilotViewModel : PilotViewModel
    {
        public IEnumerable<string> Roles { get; set; }
    }
}
