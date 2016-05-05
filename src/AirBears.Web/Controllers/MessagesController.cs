using AirBears.Web.Models;
using AirBears.Web.Services;
using AirBears.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/messages")]
    public class MessagesController : Controller
    {
        private readonly IMailer _mailer;

        public MessagesController(IMailer mailer)
        {
            _mailer = mailer;
        }

        [HttpPost]
        public async Task<IActionResult> SendContactMessage([FromBody] ContactMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
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

            await _mailer.SendAsync("airbears.uav@gmail.com", "Air Bears Web Message", message, true);
        }
    }
}