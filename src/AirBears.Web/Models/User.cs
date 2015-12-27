using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Required]
        public Guid? TeeShirtSizeId { get; set; }

        [Required]
        public string Street1 { get; set; }

        public string Street2 { get; set; }

        [Required]
        public Guid? CityId { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        [Required]
        public Guid? StateId { get; set; }

        [Required]
        public Guid? CounrtyId { get; set; }

        [Required]
        public string Zip { get; set; }

        public bool HasSignedWaiver { get; set; }

        #region Lazy Properties

        public virtual TeeShirtSize TeeShirtSize { get; set; }

        public virtual City City { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public virtual State State { get; set; }

        public virtual Country Country { get; set; }

        #endregion
    }
}
