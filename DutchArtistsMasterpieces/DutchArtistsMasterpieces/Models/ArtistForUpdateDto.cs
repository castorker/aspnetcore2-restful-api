using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DutchArtistsMasterpieces.Models
{
    public class ArtistForUpdateDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "You should provide a name value.")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "City can hold up to 100 chars.")]
        public string City { get; set; }

        [Range(0, 2018, ErrorMessage = "BirthYear must be a positive number with four digits length between 0 and 2018. e.g., 1645")]
        public int BirthYear { get; set; }

        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "", "1/1/2018")]
        public DateTime Birth { get; set; }

        [Range(0, 2018, ErrorMessage = "DeathYear must be a positive number with four digits length between 0 and 2018. e.g., 1645")]
        public int DeathYear { get; set; }

        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "", "1/1/2018")]
        public DateTime Death { get; set; }

        [StringLength(500, ErrorMessage = "Short description can hold up to 500 chars.")]
        public string ShortDescription { get; set; }

        [StringLength(5000, ErrorMessage = "Long description can hold up to 5000 chars.")]
        public string LongDescription { get; set; }

        public string ImageUrl { get; set; }
        public string ImageThumbnailUrl { get; set; }

        public bool IsArtistOfTheMonth { get; set; }
    }
}
