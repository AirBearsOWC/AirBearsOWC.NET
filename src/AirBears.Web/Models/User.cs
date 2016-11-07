using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AirBears.Web.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public Guid? TeeShirtSizeId { get; set; }

        public DateTime? TeeShirtMailedDate { get; set; }

        [MaxLength(100)]
        public string Street1 { get; set; }

        [MaxLength(100)]
        public string Street2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        /// <summary>
        /// State or province (region) depending on the country.
        /// </summary>
        public Guid? StateId { get; set; }

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

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        [MaxLength(200)]
        public string GeocodeAddress { get; set; }

        //Hoping for DbGeography in EF7 at some point in time...

        public bool HasAgreedToTerms { get; set; }

        [MaxLength(100)]
        public string Organization { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }

        /// <summary>
        /// Identifies an account as an authority account, regardless if the account has been granted the Authority role.
        /// </summary>
        public bool IsAuthorityAccount { get; set; }

        public bool AllowsPilotSearch { get; set; }

        public bool SubscribesToUpdates { get; set; }

        public bool NightVisionCapable { get; set; }

        public bool ThermalVisionCapable { get; set; }

        public bool FemaIcsCertified { get; set; }

        public bool FaaPart107Certified { get; set; }

        public bool HamRadioLicensed { get; set; }

        public bool HasInternationalAddress { get; set; }

        public Guid? PayloadId { get; set; }

        public Guid? FlightTimeId { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        #region Lazy Properties

        public virtual TeeShirtSize TeeShirtSize { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public virtual State State { get; set; }

        public virtual Payload Payload { get; set; }

        public virtual FlightTime FlightTime { get; set; }

        #endregion
    }

    public static class UserExtensions
    {
        public static string GetAddress(this User user, string state)
        {
            if(user.HasInternationalAddress)
            {
                var sb = new StringBuilder();

                sb.Append(user.AddressLine1);
                if (!string.IsNullOrWhiteSpace(user.AddressLine2)) { sb.Append($", { user.AddressLine2 }"); }
                if (!string.IsNullOrWhiteSpace(user.AddressLine3)) { sb.Append($", { user.AddressLine3 }"); }
                if (!string.IsNullOrWhiteSpace(user.AddressLine4)) { sb.Append($", { user.AddressLine4 }"); }

                return sb.ToString();
            }

            if (!string.IsNullOrWhiteSpace(user.Street2))
            {
                return $"{user.Street1}, {user.Street2}, {user.City}, {state} {user.Zip}";
            }

            return $"{user.Street1}, {user.City}, {state} {user.Zip}";
        }
    }
}
