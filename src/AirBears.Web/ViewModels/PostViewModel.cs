using System;
using System.ComponentModel.DataAnnotations;

namespace AirBears.Web.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string Slug { get; set; }

        public string Content { get; set; }

        public string FeaturedImageUrl { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime? DatePublished { get; set; }
    }
}
