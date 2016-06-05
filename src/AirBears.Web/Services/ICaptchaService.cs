using System.Threading.Tasks;

namespace AirBears.Web.Services
{
    /// <summary>
    /// Returns whether or not the Captcha response is valid.
    /// </summary>
    /// <param name="captchaResponse">Captcha response from the client.</param>
    /// <param name="userHostAddress">The client's IP address.</param>
    /// <returns></returns>
    public interface ICaptchaService
    {
        Task<bool> IsValid(string captchaResponse, string userHostAddress);
    }
}