using Newtonsoft.Json;
using System.Collections.Generic;

namespace AirBears.Web.Models
{
    /// <summary>
    /// Represents the response from Google's Recaptcha verification API.
    /// </summary>
    public class RecaptchaVerificationResponse
    {
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public ICollection<string> ErrorCodes { get; set; }
    }
}
