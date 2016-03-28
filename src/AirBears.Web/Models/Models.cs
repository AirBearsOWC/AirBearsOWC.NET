using System;
using System.ComponentModel.DataAnnotations;

// Using this Models.cs file to hold misc small models to keep folder structure flat.  Significant models
// should be placed in separate files.

namespace AirBears.Web.Models
{
    public class TeeShirtSize
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }
    }

    public class FlightTime
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }
    }

    public class Payload
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }
    }

    /// <summary>
    /// Throughout the application, state can be thought of as state or province.
    /// </summary>
    public class State
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The state's abbreviation, such as MN for Minnesota.
        /// </summary>
        public string Abbr { get; set; }
    }
}
