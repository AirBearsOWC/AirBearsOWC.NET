using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AirBears.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class PilotRegistrationViewModel : IValidatableObject
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Nonce { get; set; }

        [Required]
        [Display(Name = "T-Shirt Size")]
        public Guid? TeeShirtSizeId { get; set; }

        [Display(Name = "Street")]
        [MaxLength(100)]
        public string Street1 { get; set; }

        [MaxLength(100)]
        public string Street2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        /// <summary>
        /// State or province (region) dpending on the country.
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

        public bool HasInternationalAddress { get; set; }

        [MustBeTrue(ErrorMessage = "You must agree to the terms.")]
        public bool HasAgreedToTerms { get; set; }

        public string GetAddress(string state)
        {
            if (HasInternationalAddress)
            {
                var sb = new StringBuilder();

                sb.Append(AddressLine1);
                if (!string.IsNullOrWhiteSpace(AddressLine2)) { sb.Append($", { AddressLine2 }"); }
                if (!string.IsNullOrWhiteSpace(AddressLine3)) { sb.Append($", { AddressLine3 }"); }
                if (!string.IsNullOrWhiteSpace(AddressLine4)) { sb.Append($", { AddressLine4 }"); }

                return sb.ToString();
            }

            if (!string.IsNullOrWhiteSpace(Street2))
            {
                return $"{Street1}, {Street2}, {City}, {state} {Zip}";
            }

            return $"{Street1}, {City}, {state} {Zip}";
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HasInternationalAddress && string.IsNullOrWhiteSpace(AddressLine1)) { yield return new ValidationResult("Address Line 1 is required.", new List<string> { nameof(AddressLine1) }); }
            else if (!HasInternationalAddress)
            {
                if (string.IsNullOrWhiteSpace(Street1)) yield return new ValidationResult("Street address is required.", new List<string> { nameof(Street1) });
                if (string.IsNullOrWhiteSpace(City)) yield return new ValidationResult("City is required.", new List<string> { nameof(City) });
                if (string.IsNullOrWhiteSpace(Zip)) yield return new ValidationResult("Zip Code is required.", new List<string> { nameof(Zip) });
                if (!StateId.HasValue) yield return new ValidationResult("State is required.", new List<string> { nameof(StateId) });
            }
        }
    }

    public class AuthorityRegistrationViewModel
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
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Organization")]
        public string Organization { get; set; }

        [Required]
        public string CaptchaResponse { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string CaptchaResponse { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The new password must be at least 6 characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "new password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The new password must be at least 6 characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "new password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
