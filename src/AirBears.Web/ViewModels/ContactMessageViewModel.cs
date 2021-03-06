﻿using System.ComponentModel.DataAnnotations;

namespace AirBears.Web.ViewModels
{
    public class ContactMessageViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public string CaptchaResponse { get; set; }
    }
}
