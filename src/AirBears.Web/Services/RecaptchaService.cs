using AirBears.Web.Models;
using AirBears.Web.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirBears.Web.Services
{
    public class RecaptchaService : ICaptchaService
    {
        private RecaptchaSettings Settings { get; set; }

        public RecaptchaService(IOptions<RecaptchaSettings> settings)
        {
            Settings = settings.Value;
        }

        /// <summary>
        /// Returns whether or not the Recaptcha response is valid.
        /// </summary>
        /// <param name="captchaResponse">Recaptcha response from the client.</param>
        /// <param name="userHostAddress">The client's IP address.</param>
        /// <returns></returns>
        public async Task<bool> IsValid(string captchaResponse, string userHostAddress)
        {
            string result;
            var requestUrl = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}",
                                Settings.PrivateKey,
                                captchaResponse,
                                userHostAddress);

            using (var client = new HttpClient())
            {
                result = await client.GetStringAsync(requestUrl);
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                return false;
            }

            var obj = JsonConvert.DeserializeObject<RecaptchaVerificationResponse>(result);
            return obj.Success;
        }
    }
}
