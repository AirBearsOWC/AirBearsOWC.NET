using System.Threading.Tasks;

namespace AirBears.Web.Services
{
    public interface IMailer
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = false, bool blindCopyAppRecipient = true);
    }
}