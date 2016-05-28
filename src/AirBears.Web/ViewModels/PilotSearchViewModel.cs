using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web.ViewModels
{
    public class PilotSearchViewModel : IValidatableObject
    {
        public string Address { get; set; }

        [Range(1, 1000, ErrorMessage = "The search distance cannot exceed 1000 miles.")]
        public int Distance { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Latitude.HasValue && !Longitude.HasValue && string.IsNullOrWhiteSpace(Address))
            {
                yield return new ValidationResult("Address or coordinates are required.", new List<string> { nameof(Address) });
            }
        }
    }
}
