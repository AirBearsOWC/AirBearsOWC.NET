using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "T-Shirt Size")]
        public Guid? TeeShirtSizeId { get; set; }

        [Required]
        [Display(Name = "Street")]
        public string Street1 { get; set; }

        public string Street2 { get; set; }

        [Required]
        public string City { get; set; }

        /// <summary>
        /// State or province (region) dpending on the country.
        /// </summary>
        [Required]
        public Guid? StateId { get; set; }

        [Required]
        public string Zip { get; set; }

        [MustBeTrue(ErrorMessage = "You must agree to the terms.")]
        public bool HasAgreedToTerms { get; set; }
    }
}
