using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace AirBears.Web.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public Guid? TeeShirtSizeId { get; set; }

        public DateTime? TeeShirtMailedDate { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        /// <summary>
        /// State or province (region) depending on the country.
        /// </summary>
        public Guid? StateId { get; set; }

        public string Zip { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string GeocodeAddress { get; set; }

        //Hoping for DbGeography in EF7 at some point in time...

        public bool HasAgreedToTerms { get; set; }

        /// <summary>
        /// Identifies an account as an authority account, regardless if the account has been granted the Authority role.
        /// </summary>
        public bool IsAuthorityAccount { get; set; }

        public DateTime DateRegistered { get; set; }

        #region Lazy Properties

        public virtual TeeShirtSize TeeShirtSize { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public virtual State State { get; set; }

        #endregion
    }
}
