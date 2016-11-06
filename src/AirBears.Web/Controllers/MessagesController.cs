using AirBears.Web.Models;
using AirBears.Web.Services;
using AirBears.Web.Settings;
using AirBears.Web.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/messages")]
    public class MessagesController : Controller
    {
        private AppSettings AppSettings { get; set; }
        private readonly IMailer _mailer;
        private readonly ICaptchaService _captchaService;

        public MessagesController(IMailer mailer, ICaptchaService captchaService, IOptions<AppSettings> appSettings)
        {
            _mailer = mailer;
            _captchaService = captchaService;
            AppSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SendContactMessage([FromBody] ContactMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var remoteIpAddress = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();

            if (!await _captchaService.IsValid(model.CaptchaResponse, remoteIpAddress))
            {
                ModelState.AddModelError(string.Empty, "Failed to verify CAPTCHA. Please try again.");
                return BadRequest(ModelState);
            }

            await SendContactMessageEmail(model);

            return Ok();
        }

        private async Task SendContactMessageEmail(ContactMessageViewModel model)
        {
            var message = $"A new web contact form was submitted by { model.Name }.<br /><br />";

            message += $"<b>Name:</b> { model.Name }<br />";
            message += $"<b>Email:</b> { model.Email }<br />";

            if(!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                message += $"<b>Phone:</b> { model.PhoneNumber }<br />";
            }

            message += $"<b>Message:</b><br /><br />{ model.Message.ToHtmlWhiteSpace() }<br />";

            await _mailer.SendAsync(AppSettings.PrimaryAppRecipient, "Air Bears Web Message", message, true);
        }
    }
}