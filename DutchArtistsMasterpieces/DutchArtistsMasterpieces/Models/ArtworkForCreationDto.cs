using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DutchArtistsMasterpieces.Models
{
    public class ArtworkForCreationDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "You should provide a title value.")]
        public string Title { get; set; }

        [Range(0, 2018, ErrorMessage = "Year must be a positive number with four digits length between 0 and 2018. e.g., 1645")]
        public int Year { get; set; }

        [StringLength(500, ErrorMessage = "Short description can hold up to 500 chars.")]
        public string ShortDescription { get; set; }

        [StringLength(5000, ErrorMessage = "Long description can hold up to 5000 chars.")]
        public string LongDescription { get; set; }

        public string ImageUrl { get; set; }
        public string ImageThumbnailUrl { get; set; }

        public string Source { get; set; }
    }
}
