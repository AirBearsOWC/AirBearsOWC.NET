using System.Threading.Tasks;

namespace AirBears.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
