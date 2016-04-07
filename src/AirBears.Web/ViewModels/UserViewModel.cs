using AirBears.Web.Models;
using System;
using System.Collections.Generic;

namespace AirBears.Web.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime DateRegistered { get; set; }
    }

    public class PilotViewModel : UserViewModel
    {
        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public State State { get; set; }

        public string Zip { get; set; }

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
