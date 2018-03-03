using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchArtistsMasterpieces.Models
{
    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int BirthYear { get; set; }
        public DateTime Birth { get; set; }
        public int DeathYear { get; set; }
        public DateTime Death { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImageUrl { get; set; }
        public string ImageThumbnailUrl { get; set; }
        public bool IsArtistOfTheMonth { get; set; }

        public int NumberOfArtworks
        {
            get
            {
                return Artworks.Count;
            }
        }

        public ICollection<ArtworkDto> Artworks { get; set; } = new List<ArtworkDto>();
    }
}
