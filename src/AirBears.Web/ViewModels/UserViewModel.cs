﻿using AirBears.Web.Models;
using System.Collections.Generic;

namespace AirBears.Web.ViewModels
{
    public class UserViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public State State { get; set; }

        public string Zip { get; set; }

        public bool HasAgreedToTerms { get; set; }

        /// <summary>
        /// Identifies an account as an authority account, regardless if the account has been granted the Authority role.
        /// </summary>
        public bool IsAuthorityAccount { get; set; }

        public string TeeShirtSize { get; set; }
    }

    public class IdentityViewModel : UserViewModel
    {
        public IEnumerable<string> Roles { get; set; }
    }
}
