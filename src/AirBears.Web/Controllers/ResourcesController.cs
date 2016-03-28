using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using AirBears.Web.Models;

namespace AirBears.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/resources")]
    public class ResourcesController : Controller
    {
        private AppDbContext _context;

        public ResourcesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("states")]
        public async Task<IEnumerable<State>> GetStates()
        {
            return await _context.States.OrderBy(s => s.Name).ToListAsync();
        }

        [HttpGet("tee-shirt-sizes")]
        public async Task<IEnumerable<TeeShirtSize>> GetTeeShirtSizes()
        {
            return await _context.TeeShirtSizes.OrderBy(t => t.SortOrder).ToListAsync();
        }

        [HttpGet("payloads")]
        public async Task<IEnumerable<Payload>> GetPayloads()
        {
            return await _context.Payloads.OrderBy(t => t.SortOrder).ToListAsync();
        }

        [HttpGet("flight-times")]
        public async Task<IEnumerable<FlightTime>> GetFlightTimes()
        {
            return await _context.FlightTimes.OrderBy(t => t.SortOrder).ToListAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}