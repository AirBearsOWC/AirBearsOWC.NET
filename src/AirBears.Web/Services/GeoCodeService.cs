using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirBears.Web.Services
{
    public class GeocodeService : IGeocodeService
    {
        public async Task<CoordinateLookupResult> GetCoordsForAddress(string address)
        {
            var result = new CoordinateLookupResult();
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
                Uri.EscapeDataString(address),
                "AIzaSyB9ta5nla7x1H_Uo4HWbVT91rAFIMQ40Jo");
                
            using (var client = new HttpClient())
            {
                var request = await client.GetAsync(requestUri);
                var content = await request.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(content);
                var firstLoc = resp.results.FirstOrDefault();

                result.Status = resp.status;

                if (firstLoc != null)
                {
                    result.Latitude = firstLoc.geometry.location.lat;
                    result.Longitude = firstLoc.geometry.location.lng;
                    result.GeocodeAddress = firstLoc.formatted_address;
                }
            }

            return result;
        }

        //public static DbGeography ConvertLatLonToDbGeography(double longitude, double latitude)
        //{
        //    var point = string.Format("POINT({1} {0})", latitude, longitude);
        //    return DbGeography.FromText(point);
        //}
    }

    public class CoordinateLookupResult
    {
        public GeocodeResponseStatus Status { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string GeocodeAddress { get; set; }
    }

    #region Google GeoCode Models

    public class GoogleGeocodeResponse
    {
        public GeocodeResponseStatus status { get; set; }
        public List<Results> results { get; set; }
    }

    //"OK" indicates that no errors occurred; the address was successfully parsed and at least one geocode was returned.
    //"ZERO_RESULTS" indicates that the geocode was successful but returned no results. This may occur if the geocoder was passed a non-existent address.
    //"OVER_QUERY_LIMIT" indicates that you are over your quota.
    //"REQUEST_DENIED" indicates that your request was denied.
    //"INVALID_REQUEST" generally indicates that the query (address, components or latlng) is missing.
    //"UNKNOWN_ERROR" indicates that the request could not be processed due to a server error.The request may succeed if you try again.

    public enum GeocodeResponseStatus
    {
        OK,
        ZERO_RESULTS,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        INVALID_REQUEST,
        UNKNOWN_ERROR
    }

    public class Results
    {
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public List<string> types { get; set; }
        public List<AddressComponent> address_components { get; set; }
    }

    public class Geometry
    {
        public string location_type { get; set; }
        public Location location { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    #endregion
}
