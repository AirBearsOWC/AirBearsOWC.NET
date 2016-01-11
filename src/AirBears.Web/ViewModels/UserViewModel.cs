﻿using AirBears.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web.ViewModels
{
    public class UserViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        /// <summary>
        /// State or province depending on the country.
        /// </summary>
        public State State { get; set; }

        public string Zip { get; set; }

        public bool HasAgreedToTerms { get; set; }

        public string TeeShirtSize { get; set; }
    }
}