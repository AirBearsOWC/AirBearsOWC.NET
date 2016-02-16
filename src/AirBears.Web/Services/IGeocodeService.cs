using System.Threading.Tasks;

namespace AirBears.Web.Services
{
    public interface IGeocodeService
    {
        Task<CoordinateLookupResult> GetCoordsForAddress(string address);
    }
}